using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Events;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Inventory.Entities;
using Shop.Module.MQ;
using Shop.Module.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductOption> _productOptionRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IRepository<StockHistory> _stockHistoryRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IMQService _mqService;
        private readonly IRepository<ProductRecentlyViewed> _recentlyViewedProductRepository;

        public ProductService(
            IRepository<ProductOption> productOptionRepository,
            IRepository<Product> productRepository,
            IWorkContext workContext,
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            ICategoryService categoryService,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Stock> stockRepository,
            IRepository<StockHistory> stockHistoryRepository,
            IStaticCacheManager staticCacheManager,
            IMQService mqService,
            IRepository<ProductRecentlyViewed> recentlyViewedProductRepository)
        {
            _productOptionRepository = productOptionRepository;
            _productRepository = productRepository;
            _workContext = workContext;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _categoryService = categoryService;
            _orderItemRepository = orderItemRepository;
            _stockRepository = stockRepository;
            _stockHistoryRepository = stockHistoryRepository;
            _staticCacheManager = staticCacheManager;
            _mqService = mqService;
            _recentlyViewedProductRepository = recentlyViewedProductRepository;
        }

        public async Task<StandardTableResult<GoodsListResult>> HomeList(StandardTableParam<GoodsListQueryParam> param)
        {
            var query = _productRepository.Query();
            var search = param.Search;
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(c => c.Name.Contains(search.Name.Trim()));
                }
                if (search.CategoryId.HasValue && search.CategoryId.Value > 0)
                {
                    var ids = new List<int>();
                    ids.Add(search.CategoryId.Value);
                    //递归获取子分类
                    var all = await _categoryService.GetAll();
                    ids.AddRange(_categoryService.GetChildrens(search.CategoryId.Value, all).Select(c => c.Id));
                    var subQuery = from c in query
                                   join b in _productCategoryRepository.Query() on c.Id equals b.ProductId
                                   where ids.Distinct().Contains(b.CategoryId)
                                   select c.Id;
                    query = query.Where(c => subQuery.Distinct().Contains(c.Id));
                }
            }
            var gridData = await query
                .Include(x => x.ThumbnailImage)
                .Where(c => c.IsPublished && c.IsVisibleIndividually)
                .ToStandardTableResult(param, x => new GoodsListResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Price = x.Price,
                    OldPrice = x.OldPrice,
                    SpecialPrice = x.SpecialPrice,
                    SpecialPriceStart = x.SpecialPriceStart,
                    SpecialPriceEnd = x.SpecialPriceEnd,
                    IsAllowToOrder = x.IsAllowToOrder,
                    ThumbnailUrl = x.ThumbnailImage.Url,
                    ReviewsCount = x.ReviewsCount,
                    RatingAverage = x.RatingAverage,
                    ShortDescription = x.ShortDescription,
                    IsPublished = x.IsPublished,
                    IsFeatured = x.IsFeatured
                });
            return gridData;
        }

        public async Task<IList<GoodsListResult>> RelatedList(int id)
        {
            // 推荐商品暂时随机取商品
            var result = await _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Where(c => c.IsPublished && c.IsAllowToOrder && c.IsVisibleIndividually)
                .OrderBy(c => Guid.NewGuid())
                .Select(x => new GoodsListResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Price = x.Price,
                    OldPrice = x.OldPrice,
                    SpecialPrice = x.SpecialPrice,
                    SpecialPriceStart = x.SpecialPriceStart,
                    SpecialPriceEnd = x.SpecialPriceEnd,
                    IsAllowToOrder = x.IsAllowToOrder,
                    ThumbnailUrl = x.ThumbnailImage.Url,
                    ReviewsCount = x.ReviewsCount,
                    RatingAverage = x.RatingAverage,
                    ShortDescription = x.ShortDescription,
                    IsPublished = x.IsPublished,
                    IsFeatured = x.IsFeatured
                })
                .Take(6)
                .ToListAsync();
            return result;
        }

        public async Task<GoodsGetResult> GetGoodsByCache(int id)
        {
            var user = await _workContext.GetCurrentUserOrNullAsync();
            var result = await _staticCacheManager.GetAsync(CatalogKeys.GoodsById + id, async () =>
            {
                return await GetGoods(id);
            });

            if (user != null)
            {
                await _mqService.Send(QueueKeys.ProductView, new ProductViewed()
                {
                    UserId = user.Id,
                    EntityId = result.Id,
                    EntityTypeWithId = EntityTypeWithId.Product
                });
            }
            return result;
        }

        public async Task ClearGoodsCacheAndParent(int id)
        {
            _staticCacheManager.Remove(CatalogKeys.GoodsById + id);

            // 清理子商品缓存时,同时清理父级商品缓存
            var parentId = await _productRepository.Query().Where(c => c.Id == id && c.ParentGroupedProductId > 0).Select(c => c.ParentGroupedProductId).FirstOrDefaultAsync();
            if (parentId.HasValue)
                _staticCacheManager.Remove(CatalogKeys.GoodsById + parentId.Value);
        }

        public async Task<GoodsGetResult> GetGoods(int id)
        {
            var anyProduct = await _productRepository.Query().AnyAsync(c => c.Id == id && c.IsPublished);
            if (!anyProduct)
                throw new Exception("商品不存在或商品已下架");

            var product = await _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Include(x => x.Brand)
                .Include(x => x.Unit)
                .Include(x => x.Medias).ThenInclude(m => m.Media)
                .Include(x => x.Childrens).ThenInclude(c => c.OptionCombinations)
                .Include(x => x.OptionValues).ThenInclude(o => o.Option)
                .Include(x => x.OptionValues).ThenInclude(m => m.Media)
                .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
                .Include(x => x.Categories)
                .Include(x => x.ParentProduct)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsPublished); // 子商品必须已发布 && x.Childrens.Any(c => c.IsPublished) 这样查询是错误的

            if (product == null)
                throw new Exception("商品不存在或商品已下架");

            var productVm = new GoodsGetResult
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Sku = product.Sku,
                Gtin = product.Gtin,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Specification = product.Specification,
                OldPrice = product.OldPrice,
                Price = product.Price,
                SpecialPrice = product.SpecialPrice,
                SpecialPriceStart = product.SpecialPriceStart,
                SpecialPriceEnd = product.SpecialPriceEnd,
                IsFeatured = product.IsFeatured,
                IsPublished = product.IsPublished,
                IsAllowToOrder = product.IsAllowToOrder,
                BrandId = product.BrandId,
                ParentGroupedProductId = product.ParentGroupedProductId ?? 0,
                MediaId = product.ThumbnailImageId,
                MediaUrl = product.ThumbnailImage?.Url,
                CategoryIds = product.Categories.Select(x => x.CategoryId).ToList(),
                Barcode = product.Barcode,
                DeliveryTime = product.DeliveryTime,
                PublishedOn = product.PublishedOn,
                OrderMaximumQuantity = product.OrderMaximumQuantity,
                OrderMinimumQuantity = product.OrderMinimumQuantity,
                NotReturnable = product.NotReturnable,
                Height = product.Height,
                Length = product.Length,
                Weight = product.Weight,
                Width = product.Width,
                UnitId = product.UnitId,
                StockTrackingIsEnabled = product.StockTrackingIsEnabled,
                CreatedOn = product.CreatedOn,
                BrandName = product.Brand?.Name,
                UnitName = product.Unit?.Name
            };

            var productImages = product.Medias.Where(x => x.Media.MediaType == MediaType.Image).OrderBy(c => c.DisplayOrder).Take(10);
            foreach (var productMedia in productImages)
            {
                productVm.ProductImages.Add(new ProductGetMediaResult
                {
                    Id = productMedia.Id,
                    DisplayOrder = productMedia.DisplayOrder,
                    ProductId = product.Id,
                    Caption = productMedia.Media.Caption,
                    MediaId = productMedia.MediaId,
                    MediaUrl = productMedia.Media.Url
                });
            }
            // 添加主图
            if (product.ThumbnailImage != null && product.ThumbnailImageId.HasValue)
            {
                var first = productVm.ProductImages.FirstOrDefault(c => c.MediaId == product.ThumbnailImageId.Value);
                if (first != null)
                    productVm.ProductImages.Remove(first);
                productVm.ProductImages.Insert(0, new ProductGetMediaResult()
                {
                    Id = -1,
                    DisplayOrder = -9999,
                    MediaId = product.ThumbnailImageId.Value,
                    MediaUrl = product.ThumbnailImage?.Url,
                    ProductId = product.Id
                });
            }

            var opIds = product.OptionValues.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Option.Name).GroupBy(c => c.OptionId).Select(c => c.Key).OrderBy(c => c);
            foreach (var optionId in opIds)
            {
                var first = product.OptionValues.First(c => c.OptionId == optionId);
                var result = new ProductGetOptionResult()
                {
                    Id = first.OptionId,
                    Name = first.Option.Name,
                    DisplayType = first.Option.DisplayType,
                    Values = product.OptionValues.Where(c => c.OptionId == optionId).Select(x => new ProductGetOptionValueResult
                    {
                        Id = x.Id,
                        Display = x.Display,
                        DisplayOrder = x.DisplayOrder,
                        IsDefault = x.IsDefault,
                        MediaId = x.MediaId,
                        MediaUrl = x.Media?.Url,
                        Value = x.Value
                    }).OrderBy(c => c.DisplayOrder).ToList()
                };
                productVm.Options.Add(result);
            }

            // 子商品必须已发布 过滤方法待优化
            productVm.Variations = product.Childrens.Where(c => c.IsPublished).Select(x =>
            new ProductGetVariationResult
            {
                Id = x.Id,
                Name = x.Name,
                Sku = x.Sku,
                Gtin = x.Gtin,
                Price = x.Price,
                OldPrice = x.OldPrice,
                NormalizedName = x.NormalizedName,
                MediaId = x.ThumbnailImageId,
                MediaUrl = x.ThumbnailImage?.Url,
                //StockQuantity = x.Stock.StockQuantity,
                OptionCombinations = x.OptionCombinations.Select(p => new ProductGetOptionCombinationResult
                {
                    OptionId = p.OptionId,
                    OptionName = p.Option?.Name,
                    Value = p.Value,
                    DisplayOrder = p.DisplayOrder
                }).OrderBy(p => p.DisplayOrder).ToList(),
            }).ToList();

            var attrIds = product.AttributeValues.GroupBy(c => c.AttributeId).Select(c => c.Key);
            foreach (var attributeId in attrIds)
            {
                var list = product.AttributeValues.Where(c => c.AttributeId == attributeId);
                if (list.Count() > 0)
                {
                    var key = list.First().Attribute?.Name;
                    var values = list.OrderBy(c => c.Value).Select(c => c.Value);
                    productVm.Attributes.Add(new GoodsGetAttributeResult() { Key = key, Value = string.Join(",", values) });
                }
            }
            return productVm;
        }

        public async Task<IList<GoodsGetStockResult>> GetGoodsStocks(int id)
        {
            var anyProduct = await _productRepository.Query().AnyAsync(c => c.Id == id && c.IsPublished);
            if (!anyProduct)
                throw new Exception("产品不存在或产品已下架");

            var product = await _productRepository.Query()
                .Include(x => x.OptionCombinations)
                .Include(x => x.Childrens).ThenInclude(c => c.OptionCombinations)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsPublished);

            if (product == null)
                throw new Exception("产品不存在或产品已下架");

            var productIds = new List<int>() { id };
            productIds.AddRange(product.Childrens.Select(c => c.Id));
            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();

            var productVm = new GoodsGetStockResult
            {
                StockTrackingIsEnabled = product.StockTrackingIsEnabled,
                IsAllowToOrder = product.IsAllowToOrder,
                ProductId = product.Id,
                DisplayStockAvailability = product.DisplayStockAvailability,
                DisplayStockQuantity = product.DisplayStockQuantity,
                StockQuantity = stocks.Where(c => c.IsEnabled && c.ProductId == id).Sum(c => c.StockQuantity),
                OptionCombinations = product.OptionCombinations.Select(p => new ProductGetOptionCombinationResult
                {
                    OptionId = p.OptionId,
                    OptionName = p.Option?.Name,
                    Value = p.Value,
                    DisplayOrder = p.DisplayOrder
                }).OrderBy(p => p.DisplayOrder).ToList(),
            };

            var childrenStocks = product.Childrens.Where(c => c.IsPublished).Select(x => new GoodsGetStockResult
            {
                StockTrackingIsEnabled = x.StockTrackingIsEnabled,
                IsAllowToOrder = x.IsAllowToOrder,
                ProductId = x.Id,
                DisplayStockAvailability = x.DisplayStockAvailability,
                DisplayStockQuantity = x.DisplayStockQuantity,
                StockQuantity = stocks.Where(c => c.IsEnabled && c.ProductId == x.Id).Sum(c => c.StockQuantity),
                OptionCombinations = x.OptionCombinations.Select(p => new ProductGetOptionCombinationResult
                {
                    OptionId = p.OptionId,
                    OptionName = p.Option?.Name,
                    Value = p.Value,
                    DisplayOrder = p.DisplayOrder
                }).OrderBy(p => p.DisplayOrder).ToList(),
            }).ToList();

            var list = new List<GoodsGetStockResult>
            {
                productVm
            };
            list.AddRange(childrenStocks);

            list.ForEach(c =>
            {
                if (c.IsAllowToOrder && c.StockTrackingIsEnabled)
                {
                    if (c.StockQuantity <= 0)
                    {
                        c.IsAllowToOrder = false;
                    }
                    else
                    {
                        if (!c.DisplayStockQuantity)
                        {
                            c.StockQuantity = 0; // 不显示库存量
                        }
                    }
                }
            });
            return list;
        }
    }
}
