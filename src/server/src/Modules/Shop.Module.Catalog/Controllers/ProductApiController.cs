using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Models;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Inventory.Entities;
using Shop.Module.Orders.Entities;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 商品管理API控制器，负责商品的增删改查等管理操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("/api/products")]
    public class ProductApiController : ControllerBase
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
        private readonly IProductService _productService;

        public ProductApiController(
            IRepository<ProductOption> productOptionRepository,
            IRepository<Product> productRepository,
            IWorkContext workContext,
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            ICategoryService categoryService,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Stock> stockRepository,
            IRepository<StockHistory> stockHistoryRepository,
            IProductService productService)
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
            _productService = productService;
        }

        /// <summary>
        /// 分页获取商品列表，支持通过商品名称、SKU等条件进行筛选。
        /// </summary>
        /// <param name="param">包含分页和筛选参数的对象。</param>
        /// <returns>返回分页的商品列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<ProductQueryResult>>> List([FromBody] StandardTableParam<ProductQueryParam> param)
        {
            var query = _productRepository.Query();
            var search = param.Search;
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(c => c.Name.Contains(search.Name.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(search.Sku))
                {
                    query = query.Where(c => c.Sku.Contains(search.Sku.Trim()));
                }
                if (search.HasOptions != null)
                {
                    query = query.Where(c => c.HasOptions == search.HasOptions.Value);
                }
                if (search.IsAllowToOrder != null)
                {
                    query = query.Where(c => c.IsAllowToOrder == search.IsAllowToOrder.Value);
                }
                if (search.IsFeatured != null)
                {
                    query = query.Where(c => c.IsFeatured == search.IsFeatured.Value);
                }
                if (search.IsPublished != null)
                {
                    query = query.Where(c => c.IsPublished == search.IsPublished.Value);
                }
                if (search.IsVisibleIndividually != null)
                {
                    query = query.Where(c => c.IsVisibleIndividually == search.IsVisibleIndividually.Value);
                }
                if (search.CategoryIds.Count > 0)
                {
                    var ids = new List<int>();
                    ids.AddRange(search.CategoryIds);

                    if (search.IncludeSubCategories)
                    {
                        //递归获取子分类
                        var all = await _categoryService.GetAll();
                        foreach (var id in search.CategoryIds)
                        {
                            ids.AddRange(_categoryService.GetChildrens(id, all).Select(c => c.Id));
                        }
                    }
                    var subQuery = from c in query
                                   join b in _productCategoryRepository.Query() on c.Id equals b.ProductId
                                   where ids.Distinct().Contains(b.CategoryId)
                                   select c.Id;
                    query = query.Where(c => subQuery.Distinct().Contains(c.Id));
                }
            }
            var gridData = await query
                //.Include(x => x.Stock)
                .ToStandardTableResult(param, x => new ProductQueryResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    HasOptions = x.HasOptions,
                    IsVisibleIndividually = x.IsVisibleIndividually,
                    IsFeatured = x.IsFeatured,
                    IsAllowToOrder = x.IsAllowToOrder,
                    IsCallForPricing = x.IsCallForPricing,
                    //StockQuantity = x.Stock.StockQuantity,
                    CreatedOn = x.CreatedOn,
                    IsPublished = x.IsPublished,
                    Price = x.Price,
                    MediaUrl = x.ThumbnailImage != null ? x.ThumbnailImage.Url : null
                });
            return Result.Ok(gridData);
        }

        /// <summary>
        /// 根据商品ID获取商品详细信息，包括商品媒体、属性、库存等信息。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <returns>返回指定商品的详细信息。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result<ProductGetResult>> Get(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Include(x => x.Medias).ThenInclude(m => m.Media)
                .Include(x => x.Childrens).ThenInclude(c => c.OptionCombinations).ThenInclude(c => c.Option)
                .Include(x => x.Childrens)
                .Include(x => x.OptionValues).ThenInclude(o => o.Option)
                .Include(x => x.OptionValues).ThenInclude(m => m.Media)
                .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
                throw new Exception("商品不存在");

            var productIds = new List<int>() { id };
            productIds.AddRange(product.Childrens.Select(c => c.Id));
            var stocks = await _stockRepository.Query().Include(c => c.Warehouse).Where(c => productIds.Contains(c.ProductId)).ToListAsync();

            var productVm = new ProductGetResult
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                MetaTitle = product.MetaTitle,
                MetaKeywords = product.MetaKeywords,
                MetaDescription = product.MetaDescription,
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
                IsCallForPricing = product.IsCallForPricing,
                IsAllowToOrder = product.IsAllowToOrder,
                StockTrackingIsEnabled = product.StockTrackingIsEnabled,
                BrandId = product.BrandId,
                ParentGroupedProductId = product.ParentGroupedProductId ?? 0,
                MediaId = product.ThumbnailImageId,
                MediaUrl = product.ThumbnailImage?.Url,
                CategoryIds = product.Categories.Select(x => x.CategoryId).ToList(),
                Barcode = product.Barcode,
                DeliveryTime = product.DeliveryTime,
                ValidThru = product.ValidThru,
                PublishType = product.PublishType,
                PublishedOn = product.PublishedOn,
                OrderMaximumQuantity = product.OrderMaximumQuantity,
                OrderMinimumQuantity = product.OrderMinimumQuantity,
                DisplayStockAvailability = product.DisplayStockAvailability,
                DisplayStockQuantity = product.DisplayStockQuantity,
                NotReturnable = product.NotReturnable,
                StockReduceStrategy = product.StockReduceStrategy,
                UnpublishedOn = product.UnpublishedOn,
                UnpublishedReason = product.UnpublishedReason,
                IsVisibleIndividually = product.IsVisibleIndividually,
                AdditionalShippingCharge = product.AdditionalShippingCharge,
                AdminRemark = product.AdminRemark,
                FreightTemplateId = product.FreightTemplateId,
                Height = product.Height,
                IsFreeShipping = product.IsFreeShipping,
                IsShipEnabled = product.IsShipEnabled,
                Length = product.Length,
                Weight = product.Weight,
                Width = product.Width,
                UnitId = product.UnitId
            };

            productVm.StockQuantity = stocks.Where(c => c.IsEnabled && c.ProductId == id).Sum(c => c.StockQuantity);
            productVm.WarehouseIds = stocks.Where(c => c.ProductId == id).Select(c => c.WarehouseId).ToList();
            productVm.Stocks = stocks.Where(c => c.ProductId == id).Select(c => new ProductGetStockResult()
            {
                DisplayOrder = c.DisplayOrder,
                Id = c.WarehouseId,
                IsEnabled = c.IsEnabled,
                Name = c.Warehouse.Name,
                Quantity = c.StockQuantity
            }).OrderBy(c => c.DisplayOrder).ToList();

            foreach (var productMedia in product.Medias.Where(x => x.Media.MediaType == MediaType.Image))
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

            productVm.Variations = product.Childrens.Select(x =>
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
                StockQuantity = stocks.Where(c => c.IsEnabled && c.ProductId == x.Id).Sum(c => c.StockQuantity),
                WarehouseIds = stocks.Where(c => c.ProductId == x.Id).Select(c => c.WarehouseId).ToList(),
                Stocks = stocks.Where(c => c.ProductId == x.Id).Select(c => new ProductGetStockResult()
                {
                    DisplayOrder = c.DisplayOrder,
                    Id = c.WarehouseId,
                    IsEnabled = c.IsEnabled,
                    Name = c.Warehouse.Name,
                    Quantity = c.StockQuantity
                }).OrderBy(c => c.DisplayOrder).ToList()
            }).ToList();

            var attrIds = product.AttributeValues.GroupBy(c => c.AttributeId).Select(c => c.Key);
            foreach (var attributeId in attrIds)
            {
                var list = product.AttributeValues.Where(c => c.AttributeId == attributeId);
                productVm.Attributes.Add(new ProductGetAttributeResult
                {
                    Id = attributeId,
                    Name = list.First().Attribute.Name,
                    Values = list.Select(c => new ProductGetAttributeValueResult()
                    {
                        Id = c.Id,
                        Value = c.Value
                    }).ToList()
                });
            }
            return Result.Ok(productVm);
        }

        /// <summary>
        /// 添加新商品，支持设置商品的各种属性和关联信息。
        /// </summary>
        /// <param name="param">包含商品创建信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPost]
        public async Task<Result> Add([FromBody] ProductCreateParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = new Product
            {
                Name = param.Name,
                Slug = param.Slug,
                MetaTitle = param.MetaTitle,
                MetaKeywords = param.MetaKeywords,
                MetaDescription = param.MetaDescription,
                Sku = param.Sku,
                Gtin = param.Gtin,
                ShortDescription = param.ShortDescription,
                Description = param.Description,
                Specification = param.Specification,
                Price = param.Price,
                OldPrice = param.OldPrice,
                SpecialPrice = param.SpecialPrice,
                SpecialPriceStart = param.SpecialPriceStart,
                SpecialPriceEnd = param.SpecialPriceEnd,
                IsPublished = param.IsPublished,
                IsFeatured = param.IsFeatured,
                IsCallForPricing = param.IsCallForPricing,
                IsAllowToOrder = param.IsAllowToOrder,
                BrandId = param.BrandId,
                StockTrackingIsEnabled = param.StockTrackingIsEnabled,
                ThumbnailImageId = param.ThumbnailImageUrlId,

                CreatedBy = currentUser,
                UpdatedBy = currentUser,
                ParentGroupedProductId = null,
                IsVisibleIndividually = true, //新增产品时，必须可见
                HasOptions = param.Variations.Distinct().Any() ? true : false,

                Barcode = param.Barcode,
                DeliveryTime = param.DeliveryTime,
                ValidThru = param.ValidThru,
                //DefaultWarehouseId = param.WarehouseId,
                PublishType = param.PublishType,
                OrderMaximumQuantity = param.OrderMaximumQuantity,
                OrderMinimumQuantity = param.OrderMinimumQuantity,
                DisplayStockAvailability = param.DisplayStockAvailability,
                DisplayStockQuantity = param.DisplayStockQuantity,
                NotReturnable = param.NotReturnable,
                StockReduceStrategy = param.StockReduceStrategy,
                UnpublishedOn = param.UnpublishedOn,
                UnpublishedReason = param.UnpublishedReason,
                //StockQuantity = param.StockQuantity,
                //IsVisibleIndividually = param.IsVisibleIndividually

                AdditionalShippingCharge = param.AdditionalShippingCharge,
                AdminRemark = param.AdminRemark,
                FreightTemplateId = param.FreightTemplateId,
                Height = param.Height,
                IsFreeShipping = param.IsFreeShipping,
                IsShipEnabled = param.IsShipEnabled,
                Length = param.Length,
                Weight = param.Weight,
                Width = param.Width,
                UnitId = param.UnitId
            };

            if (param.PublishType == PublishType.Now || param.IsPublished)
            {
                product.PublishedOn = DateTime.Now;
                product.IsPublished = true;
            }
            else
            {
                product.PublishedOn = param.PublishedOn;
            }

            foreach (var option in param.Options.Distinct())
            {
                foreach (var item in option.Values.Distinct())
                {
                    product.AddOptionValue(new ProductOptionValue
                    {
                        OptionId = option.Id,
                        Display = item.Display,
                        Value = item.Value,
                        IsDefault = item.IsDefault,
                        MediaId = item.MediaId,
                        DisplayOrder = item.DisplayOrder
                    });
                }
            }

            foreach (var categoryId in param.CategoryIds.Distinct())
            {
                var productCategory = new ProductCategory
                {
                    CategoryId = categoryId
                };
                product.AddCategory(productCategory);
            }

            foreach (var mediaId in param.MediaIds.Distinct())
            {
                var productMedia = new ProductMedia
                {
                    MediaId = mediaId
                };
                product.AddMedia(productMedia);
            }

            foreach (var attribute in param.Attributes.Distinct())
            {
                foreach (var value in attribute.Values.Distinct())
                {
                    var attributeValue = new ProductAttributeValue
                    {
                        AttributeId = attribute.AttributeId,
                        Value = value
                    };
                    product.AddAttributeValue(attributeValue);
                }
            }
            product.AddPriceHistory(currentUser);

            var stocks = new List<Stock>();
            var stockHistories = new List<StockHistory>();
            if (product.StockTrackingIsEnabled)
            {
                InitStock(stocks, stockHistories, product, currentUser, param.Stocks);
            }

            //商品选项组合
            var variations = param.Variations.Distinct();
            foreach (var variation in variations)
            {
                var child = product.Clone();
                child.ParentProduct = product;

                child.Sku = variation.Sku;
                child.Name = variation.Name;
                child.NormalizedName = variation.NormalizedName;
                child.Gtin = variation.Gtin;
                child.Price = variation.Price;
                child.OldPrice = variation.OldPrice;

                child.HasOptions = false;
                child.IsVisibleIndividually = false;

                child.CreatedById = currentUser.Id;
                child.UpdatedById = currentUser.Id;

                child.ThumbnailImageId = variation.MediaId;

                if (child.StockTrackingIsEnabled)
                {
                    InitStock(stocks, stockHistories, child, currentUser, variation.Stocks);
                }

                var coms = variation.OptionCombinations.Distinct();
                foreach (var combination in coms)
                {
                    if (!product.OptionValues.Any(c => c.OptionId == combination.OptionId))
                        throw new Exception("商品组合中的选项不存在");
                    if (!product.OptionValues.Any(c => c.Value == combination.Value))
                        throw new Exception("商品组合中的选项值不存在");
                    if (product.OptionCombinations.Any(c => c.OptionId == combination.OptionId && c.Value == combination.Value))
                        continue;

                    child.AddOptionCombination(new ProductOptionCombination
                    {
                        OptionId = combination.OptionId,
                        Value = combination.Value,
                        DisplayOrder = combination.DisplayOrder
                    });
                }

                child.AddPriceHistory(currentUser);
                product.Childrens.Add(child);
            }
            _productRepository.Add(product);

            using (var transaction = _productRepository.BeginTransaction())
            {
                await _productRepository.SaveChangesAsync();

                if (stocks.Count > 0)
                {
                    _stockRepository.AddRange(stocks);
                    await _stockRepository.SaveChangesAsync();
                }
                if (stockHistories.Count > 0)
                {
                    _stockHistoryRepository.AddRange(stockHistories);
                    await _stockHistoryRepository.SaveChangesAsync();
                }
                transaction.Commit();
            }
            return Result.Ok();
        }

        /// <summary>
        /// 编辑指定ID的商品信息，支持修改商品的各种属性和关联信息。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <param name="param">包含商品更新信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Edit(int id, [FromBody] ProductCreateParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Include(x => x.Medias).ThenInclude(m => m.Media)
                .Include(x => x.Childrens).ThenInclude(c => c.OptionCombinations)
                .Include(x => x.Childrens)
                .Include(x => x.OptionValues).ThenInclude(o => o.Option)
                .Include(x => x.OptionValues).ThenInclude(m => m.Media)
                .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
                .Include(x => x.Categories)
                .Include(x => x.ParentProduct)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
                return Result.Fail("商品不存在");

            var productIds = new List<int>() { id };
            productIds.AddRange(product.Childrens.Select(c => c.Id));
            var stocks = await _stockRepository.Query().Include(c => c.Warehouse).Where(c => productIds.Contains(c.ProductId)).ToListAsync();

            var isPriceChanged = false;
            if (product.Price != param.Price ||
                product.OldPrice != param.OldPrice ||
                product.SpecialPrice != param.SpecialPrice ||
                product.SpecialPriceStart != param.SpecialPriceStart ||
                product.SpecialPriceEnd != param.SpecialPriceEnd)
            {
                isPriceChanged = true;
            }

            if (param.IsPublished)
            {
                if (!product.IsPublished)
                {
                    product.PublishedOn = DateTime.Now;
                    product.IsPublished = true;
                }
            }
            else
            {
                if (param.PublishType == PublishType.Now)
                {
                    product.PublishedOn = DateTime.Now;
                    product.IsPublished = true;
                }
                else
                {
                    product.PublishedOn = param.PublishedOn;
                    product.IsPublished = false;

                    if (product.IsPublished)
                    {
                        product.UnpublishedOn = DateTime.Now;
                    }
                }
            }

            product.Name = param.Name;
            product.Slug = param.Slug;
            product.MetaTitle = param.MetaTitle;
            product.MetaKeywords = param.MetaKeywords;
            product.MetaDescription = param.MetaDescription;
            product.Sku = param.Sku;
            product.Gtin = param.Gtin;
            product.ShortDescription = param.ShortDescription;
            product.Description = param.Description;
            product.Specification = param.Specification;
            product.Price = param.Price;
            product.OldPrice = param.OldPrice;
            product.SpecialPrice = param.SpecialPrice;
            product.SpecialPriceStart = param.SpecialPriceStart;
            product.SpecialPriceEnd = param.SpecialPriceEnd;
            product.IsFeatured = param.IsFeatured;
            product.IsCallForPricing = param.IsCallForPricing;
            product.IsAllowToOrder = param.IsAllowToOrder;
            product.BrandId = param.BrandId;
            product.StockTrackingIsEnabled = param.StockTrackingIsEnabled;
            product.ThumbnailImageId = param.ThumbnailImageUrlId;

            product.Barcode = param.Barcode;
            product.DeliveryTime = param.DeliveryTime;
            product.ValidThru = param.ValidThru;
            product.PublishType = param.PublishType;
            product.OrderMaximumQuantity = param.OrderMaximumQuantity;
            product.OrderMinimumQuantity = param.OrderMinimumQuantity;
            product.DisplayStockAvailability = param.DisplayStockAvailability;
            product.DisplayStockQuantity = param.DisplayStockQuantity;
            product.NotReturnable = param.NotReturnable;
            product.StockReduceStrategy = param.StockReduceStrategy;
            product.UnpublishedOn = param.UnpublishedOn;
            product.UnpublishedReason = param.UnpublishedReason;

            product.AdditionalShippingCharge = param.AdditionalShippingCharge;
            product.AdminRemark = param.AdminRemark;
            product.FreightTemplateId = param.FreightTemplateId;
            product.Height = param.Height;
            product.IsFreeShipping = param.IsFreeShipping;
            product.IsShipEnabled = param.IsShipEnabled;
            product.Length = param.Length;
            product.Weight = param.Weight;
            product.Width = param.Width;
            product.UnitId = param.UnitId;

            product.UpdatedOn = DateTime.Now;
            product.UpdatedBy = currentUser;

            if (isPriceChanged)
            {
                product.AddPriceHistory(currentUser);
            }

            var addStocks = new List<Stock>();
            var addStockHistories = new List<StockHistory>();
            if (product.StockTrackingIsEnabled)
            {
                var productStocks = stocks.Where(c => c.ProductId == id);

                var initStockParams = param.Stocks.Distinct().Where(c => !productStocks.Select(x => x.WarehouseId).Contains(c.Id));
                var updateStockParams = param.Stocks.Distinct().Where(c => productStocks.Select(x => x.WarehouseId).Contains(c.Id));

                InitStock(addStocks, addStockHistories, product, currentUser, initStockParams);
                UpdateAndDeleteStock(productStocks, addStockHistories, product, currentUser, updateStockParams);
            }

            //如果是编辑的商品组合（SKU），则不参与编辑选项
            //如果是编辑的主商品，则参与编辑选项
            if (product.ParentGroupedProductId == null && product.ParentProduct == null)
            {
                product.HasOptions = param.Variations.Distinct().Any() ? true : false;

                //商品选项
                //暂时全部标识删除
                foreach (var ov in product.OptionValues)
                {
                    ov.IsDeleted = true;
                    ov.UpdatedOn = DateTime.Now;
                }

                var options = param.Options.Distinct();
                foreach (var option in options)
                {
                    foreach (var item in option.Values.Distinct())
                    {
                        var ov = product.OptionValues.FirstOrDefault(c => c.OptionId == option.Id && c.Value == item.Value);
                        if (ov == null)
                        {
                            product.AddOptionValue(new ProductOptionValue
                            {
                                OptionId = option.Id,
                                Value = item.Value,
                                Display = item.Display,
                                DisplayOrder = item.DisplayOrder,
                                IsDefault = item.IsDefault,
                                MediaId = item.MediaId
                            });
                        }
                        else
                        {
                            ov.Display = item.Display;
                            ov.DisplayOrder = item.DisplayOrder;
                            ov.IsDefault = item.IsDefault;
                            ov.MediaId = item.MediaId;
                            ov.IsDeleted = false;
                        }
                    }
                }

                var deleteOvs = product.OptionValues.Where(x => options.All(c => c.Values.All(v => v.Value != x.Value) && x.OptionId != c.Id));
                foreach (var ov in deleteOvs)
                {
                    ov.IsDeleted = true;
                    ov.UpdatedOn = DateTime.Now;
                }

                //商品选项组合
                var variations = param.Variations.Distinct();
                foreach (var variation in variations)
                {
                    Product child = null;
                    if (variation.Id > 0)
                    {
                        child = product.Childrens.FirstOrDefault(c => c.Id == variation.Id);
                    }

                    if (child == null)
                    {
                        child = product.Clone();
                        child.ParentProduct = product;

                        child.Sku = variation.Sku;
                        child.Name = variation.Name;
                        child.NormalizedName = variation.NormalizedName;
                        child.Gtin = variation.Gtin;
                        child.Price = variation.Price;
                        child.OldPrice = variation.OldPrice;

                        child.HasOptions = false;
                        child.IsVisibleIndividually = false;

                        child.CreatedById = currentUser.Id;
                        child.UpdatedById = currentUser.Id;

                        child.ThumbnailImageId = variation.MediaId;

                        var coms = variation.OptionCombinations.Distinct();
                        foreach (var combination in coms)
                        {
                            if (!product.OptionValues.Any(c => c.OptionId == combination.OptionId && !c.IsDeleted))
                                throw new Exception("商品组合中的选项不存在");
                            if (!product.OptionValues.Any(c => c.Value == combination.Value && !c.IsDeleted))
                                throw new Exception("商品组合中的选项值不存在");
                            if (product.OptionCombinations.Any(c => c.OptionId == combination.OptionId && c.Value == combination.Value))
                                continue;

                            child.AddOptionCombination(new ProductOptionCombination
                            {
                                OptionId = combination.OptionId,
                                Value = combination.Value,
                                DisplayOrder = combination.DisplayOrder
                            });
                        }

                        if (child.StockTrackingIsEnabled)
                        {
                            InitStock(addStocks, addStockHistories, child, currentUser, variation.Stocks);
                        }

                        child.AddPriceHistory(currentUser);
                        product.Childrens.Add(child);
                    }
                    else
                    {
                        var childIsPriceChanged = false;
                        if (child.Price != variation.Price ||
                            child.OldPrice != variation.OldPrice)
                        {
                            childIsPriceChanged = true;
                        }

                        child.Sku = variation.Sku;
                        child.Name = variation.Name;
                        child.Gtin = variation.Gtin;
                        child.Price = variation.Price;
                        child.OldPrice = variation.OldPrice;
                        child.NormalizedName = variation.NormalizedName;
                        child.ThumbnailImageId = variation.MediaId;
                        child.UpdatedById = currentUser.Id;
                        child.UpdatedOn = DateTime.Now;

                        if (child.StockTrackingIsEnabled)
                        {
                            var childStocks = stocks.Where(c => c.ProductId == child.Id);

                            var initStockParams = variation.Stocks.Distinct().Where(c => !childStocks.Select(x => x.WarehouseId).Contains(c.Id));
                            var updateStockParams = variation.Stocks.Distinct().Where(c => childStocks.Select(x => x.WarehouseId).Contains(c.Id));

                            InitStock(addStocks, addStockHistories, child, currentUser, initStockParams);
                            UpdateAndDeleteStock(childStocks, addStockHistories, child, currentUser, updateStockParams);
                        }

                        if (childIsPriceChanged)
                        {
                            child.AddPriceHistory(currentUser);
                        }
                    }
                }

                var deleteChidrens = product.Childrens.Where(x => variations.All(c => c.Id != x.Id));
                foreach (var chidren in deleteChidrens)
                {
                    chidren.IsDeleted = true;
                    chidren.UpdatedOn = DateTime.Now;
                }

                //如果是产品下架，则SKU全部下架
                if (!product.IsPublished)
                {
                    foreach (var item in product.Childrens.Where(c => c.IsDeleted == false && c.IsPublished))
                    {
                        item.IsPublished = false;
                        item.UnpublishedOn = DateTime.Now;
                    }
                }
            }
            else
            {
                //SKU 可编辑单独可见字段
                product.IsVisibleIndividually = param.IsVisibleIndividually;
            }

            var categoryIds = param.CategoryIds.Distinct();
            foreach (var categoryId in categoryIds)
            {
                if (product.Categories.Any(x => x.CategoryId == categoryId))
                {
                    continue;
                }
                var productCategory = new ProductCategory
                {
                    CategoryId = categoryId
                };
                product.AddCategory(productCategory);
            }
            var deletedProductCategories = product.Categories.Where(productCategory => !categoryIds.Contains(productCategory.CategoryId));
            foreach (var deletedProductCategory in deletedProductCategories)
            {
                deletedProductCategory.IsDeleted = true;
                deletedProductCategory.UpdatedOn = DateTime.Now;
            }

            var mediaIds = param.MediaIds.Distinct();
            foreach (var mediaId in mediaIds)
            {
                if (product.Medias.Any(x => x.MediaId == mediaId))
                {
                    continue;
                }
                var productMedia = new ProductMedia
                {
                    MediaId = mediaId
                };
                product.AddMedia(productMedia);
            }
            var deletedProductMedias = product.Medias.Where(media => !mediaIds.Contains(media.MediaId));
            foreach (var deletedProductMedia in deletedProductMedias)
            {
                deletedProductMedia.IsDeleted = true;
                deletedProductMedia.UpdatedOn = DateTime.Now;
            }

            var attributes = param.Attributes.Distinct();
            foreach (var attribute in attributes)
            {
                var velues = attribute.Values.Distinct().Where(c => !string.IsNullOrWhiteSpace(c));
                var productAttrValues = product.AttributeValues.Where(x => x.AttributeId == attribute.AttributeId);
                foreach (var item in productAttrValues)
                {
                    if (!velues.Contains(item.Value))
                    {
                        item.IsDeleted = true;
                        item.UpdatedOn = DateTime.Now;
                    }
                }
                foreach (var value in velues)
                {
                    if (!productAttrValues.Any(c => c.Value == value))
                    {
                        var attributeValue = new ProductAttributeValue
                        {
                            AttributeId = attribute.AttributeId,
                            Value = value
                        };
                        product.AddAttributeValue(attributeValue);
                    }
                }
            }
            var deletedAttrValues = product.AttributeValues.Where(attrValue => attributes.All(x => x.AttributeId != attrValue.AttributeId));
            foreach (var deletedAttrValue in deletedAttrValues)
            {
                deletedAttrValue.IsDeleted = true;
                deletedAttrValue.UpdatedOn = DateTime.Now;
            }

            using (var transaction = _productRepository.BeginTransaction())
            {
                await _productRepository.SaveChangesAsync();

                if (addStocks.Count > 0)
                {
                    _stockRepository.AddRange(addStocks);
                }
                await _stockRepository.SaveChangesAsync();

                if (addStockHistories.Count > 0)
                {
                    _stockHistoryRepository.AddRange(addStockHistories);
                    await _stockHistoryRepository.SaveChangesAsync();
                }
                transaction.Commit();
            }
            await _productService.ClearGoodsCacheAndParent(product.Id);
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的商品，包括其关联的选项、属性、媒体等信息。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = _productRepository.Query()
            .Include(x => x.ThumbnailImage)
            .Include(x => x.Medias).ThenInclude(m => m.Media)
            .Include(x => x.OptionValues).ThenInclude(o => o.Option)
            .Include(x => x.OptionCombinations)
            .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
            .Include(x => x.Categories)
            .Include(x => x.PriceHistories)
            //.Include(x => x.Stock)
            //.Include(x => x.StockHistories)
            .FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return Result.Fail("单据不存在");
            }

            var anyChild = await _productRepository.Query().AnyAsync(c => c.ParentGroupedProductId == product.Id);
            if (anyChild)
            {
                return Result.Fail("当前商品存在商品组合，不允许删除");
            }

            //如果产品产生了订单，则暂时不允许删除产品
            var anyOrder = await _orderItemRepository.Query().AnyAsync(c => c.ProductId == product.Id);
            if (anyOrder)
            {
                return Result.Fail("当前商品已被订购，暂不允许删除");
            }

            foreach (var deletedOptionValue in product.OptionValues)
            {
                deletedOptionValue.IsDeleted = true;
                deletedOptionValue.UpdatedOn = DateTime.Now;
            }

            foreach (var combination in product.OptionCombinations)
            {
                combination.IsDeleted = true;
                combination.UpdatedOn = DateTime.Now;
            }

            foreach (var deletedProductCategory in product.Categories)
            {
                deletedProductCategory.IsDeleted = true;
                deletedProductCategory.UpdatedOn = DateTime.Now;
            }

            foreach (var deletedProductMedia in product.Medias)
            {
                deletedProductMedia.IsDeleted = true;
                deletedProductMedia.UpdatedOn = DateTime.Now;
            }

            foreach (var deletedAttrValue in product.AttributeValues)
            {
                deletedAttrValue.IsDeleted = true;
                deletedAttrValue.UpdatedOn = DateTime.Now;
            }

            foreach (var priceHistory in product.PriceHistories)
            {
                priceHistory.IsDeleted = true;
                priceHistory.UpdatedOn = DateTime.Now;
                priceHistory.UpdatedBy = currentUser;
            }

            //foreach (var stockHistory in product.StockHistories)
            //{
            //    stockHistory.IsDeleted = true;
            //    stockHistory.UpdatedOn = DateTime.Now;
            //    stockHistory.UpdatedBy = currentUser;
            //}

            //if (product.Stock != null)
            //{
            //    product.Stock.IsDeleted = true;
            //    product.Stock.UpdatedOn = DateTime.Now;
            //}

            product.IsDeleted = true;
            product.UpdatedOn = DateTime.Now;
            product.UpdatedBy = currentUser;

            await _productRepository.SaveChangesAsync();
            await _productService.ClearGoodsCacheAndParent(product.Id);
            return Result.Ok();
        }

        /// <summary>
        /// 发布指定ID的商品，使其在前台可见。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("{id:int:min(1)}/publish")]
        public async Task<Result> Publish(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = _productRepository.Query()
                .Include(c => c.ParentProduct)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
                return Result.Fail("商品不存在");

            if (product.IsPublished)
                return Result.Ok();

            if (product.ParentProduct != null && !product.ParentProduct.IsPublished)
            {
                return Result.Fail("当前商品对应的父商品未发布，不允许操作");
            }

            product.IsPublished = true;
            product.PublishedOn = DateTime.Now;

            await _productRepository.SaveChangesAsync();
            await _productService.ClearGoodsCacheAndParent(product.Id);
            return Result.Ok();
        }

        /// <summary>
        /// 取消发布指定ID的商品，使其在前台不可见。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("{id:int:min(1)}/unpublish")]
        public async Task<Result> Unpublish(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = _productRepository.Query()
                .Include(x => x.Childrens)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
                return Result.Fail("商品不存在");

            if (!product.IsPublished)
                return Result.Ok();

            product.IsPublished = false;
            product.UnpublishedOn = DateTime.Now;
            foreach (var item in product.Childrens)
            {
                item.IsPublished = false;
                item.UnpublishedOn = DateTime.Now;
            }

            await _productRepository.SaveChangesAsync();
            await _productService.ClearGoodsCacheAndParent(product.Id);
            return Result.Ok();
        }

        /// <summary>
        /// 克隆指定ID的商品，创建一个新的商品副本。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <param name="model">包含克隆商品时可选参数的对象。</param>
        /// <returns>返回操作结果，包括新克隆的商品ID。</returns>
        [HttpPost("{id:int:min(1)}/clone")]
        public async Task<Result> Clone(int id, [FromBody] ProductCloneParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var product = _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Include(x => x.Medias).ThenInclude(m => m.Media)
                .Include(x => x.OptionValues).ThenInclude(o => o.Option)
                .Include(x => x.OptionValues).ThenInclude(m => m.Media)
                .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
                throw new Exception("商品不存在");

            var newProduct = product.Clone();
            newProduct.Name = model.Name;
            newProduct.Slug = model.Slug;
            newProduct.CreatedById = currentUser.Id;
            newProduct.UpdatedById = currentUser.Id;
            newProduct.CreatedOn = DateTime.Now;
            newProduct.UpdatedOn = DateTime.Now;
            newProduct.AddPriceHistory(currentUser);

            if (model.IsCopyImages)
            {
                newProduct.ThumbnailImageId = product.ThumbnailImageId;
                foreach (var productMedia in product.Medias)
                {
                    newProduct.AddMedia(new ProductMedia
                    {
                        MediaId = productMedia.MediaId
                    });
                }
            }

            //复制选项
            foreach (var optionValue in product.OptionValues)
            {
                newProduct.AddOptionValue(new ProductOptionValue
                {
                    OptionId = optionValue.OptionId,
                    Display = optionValue.Display,
                    Value = optionValue.Value,
                    IsDefault = optionValue.IsDefault,
                    MediaId = optionValue.MediaId,
                    DisplayOrder = optionValue.DisplayOrder
                });
            }

            //复制库存
            var addStocks = new List<Stock>();
            var addStockHistories = new List<StockHistory>();
            if (model.IsCopyStock)
            {
                var stocks = await _stockRepository.Query().Include(c => c.Warehouse).Where(c => c.ProductId == product.Id).ToListAsync();
                if (newProduct.StockTrackingIsEnabled)
                {
                    var stockParams = stocks.Select(c => new ProductCreateStockParam()
                    {
                        DisplayOrder = c.DisplayOrder,
                        Id = c.WarehouseId,
                        IsEnabled = c.IsEnabled,
                        Quantity = c.StockQuantity
                    });
                    InitStock(addStocks, addStockHistories, newProduct, currentUser, stockParams);
                }
            }
            _productRepository.Add(newProduct);

            using (var transaction = _productRepository.BeginTransaction())
            {
                await _productRepository.SaveChangesAsync();

                if (addStocks.Count > 0)
                {
                    _stockRepository.AddRange(addStocks);
                    await _stockRepository.SaveChangesAsync();
                }
                if (addStockHistories.Count > 0)
                {
                    _stockHistoryRepository.AddRange(addStockHistories);
                    await _stockHistoryRepository.SaveChangesAsync();
                }
                transaction.Commit();
            }
            return Result.Ok(newProduct.Id);
        }

        private void InitStock(List<Stock> addStocks, List<StockHistory> addStockHistories, Product product, User user, IEnumerable<ProductCreateStockParam> stockParams)
        {
            if (stockParams == null || stockParams.Count() <= 0)
                return;

            foreach (var item in stockParams.Distinct())
            {
                if (item.Quantity < 0 || addStocks.Any(c => c.WarehouseId == item.Id && c.Product == product))
                    continue;

                addStocks.Add(new Stock()
                {
                    Product = product,
                    LockedStockQuantity = 0,
                    StockQuantity = item.Quantity,
                    DisplayOrder = item.DisplayOrder,
                    IsEnabled = item.IsEnabled,
                    WarehouseId = item.Id
                });
                addStockHistories.Add(new StockHistory()
                {
                    Product = product,
                    WarehouseId = item.Id,
                    StockQuantity = item.Quantity,
                    AdjustedQuantity = item.Quantity,
                    CreatedBy = user,
                    UpdatedBy = user,
                    Note = "初始化商品库存"
                });
            }
        }

        private void UpdateAndDeleteStock(IEnumerable<Stock> productStocks, List<StockHistory> addStockHistories, Product product, User user, IEnumerable<ProductCreateStockParam> stockParams)
        {
            if (stockParams == null || stockParams.Count() <= 0)
                return;

            foreach (var item in stockParams)
            {
                if (item.Quantity < 0)
                    throw new Exception("库存数量必须>=0");

                var ps = productStocks.FirstOrDefault(c => c.WarehouseId == item.Id);
                if (ps == null)
                    throw new Exception("产品库存不存在");

                if (ps.StockQuantity != item.Quantity)
                {
                    addStockHistories.Add(new StockHistory()
                    {
                        Product = product,
                        WarehouseId = item.Id,
                        StockQuantity = item.Quantity,
                        AdjustedQuantity = item.Quantity - ps.StockQuantity,
                        CreatedBy = user,
                        UpdatedBy = user,
                        Note = "修改商品库存"
                    });
                    ps.StockQuantity = item.Quantity;
                }
                ps.IsEnabled = item.IsEnabled;
                ps.DisplayOrder = item.DisplayOrder;
                ps.UpdatedOn = DateTime.Now;
            }

            var deletedProductStocks = productStocks.Where(c => !stockParams.Select(x => x.Id).Contains(c.WarehouseId));
            foreach (var item in deletedProductStocks)
            {
                item.IsDeleted = true;
                item.UpdatedOn = DateTime.Now;
            }
        }
    }
}