using Shop.Infrastructure.Models;
using Shop.Module.Catalog.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Shipping.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Module.Catalog.Entities
{
    public class Product : EntityBase
    {
        public Product()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the parent product identifier. It's used to identify associated products (only with "grouped" products)
        /// </summary>
        [ForeignKey("ParentProduct")]
        public int? ParentGroupedProductId { get; set; }

        [ForeignKey("ParentGroupedProductId")]
        public Product ParentProduct { get; set; }

        public IList<Product> Childrens { get; set; } = new List<Product>();

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        [Required]
        [StringLength(450)]
        public string Slug { get; set; }

        public string MetaTitle { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string Specification { get; set; }

        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public decimal? SpecialPrice { get; set; }

        public DateTime? SpecialPriceStart { get; set; }

        public DateTime? SpecialPriceEnd { get; set; }

        public bool HasOptions { get; set; }

        /// <summary>
        /// Gets or sets the values indicating whether this product is visible in catalog or search results.
        /// It's used when this product is associated to some "grouped" one
        /// This way associated products could be accessed/added/etc only from a grouped product details page
        /// </summary>
        public bool IsVisibleIndividually { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsCallForPricing { get; set; }

        public bool IsAllowToOrder { get; set; }

        public bool StockTrackingIsEnabled { get; set; }

        [StringLength(450)]
        public string Sku { get; set; }

        [StringLength(450)]
        public string Gtin { get; set; }

        [StringLength(450)]
        public string NormalizedName { get; set; }

        public Media ThumbnailImage { get; set; }

        public int? ThumbnailImageId { get; set; }

        public int ReviewsCount { get; set; }

        public double? RatingAverage { get; set; }

        public int? BrandId { get; set; }

        public Brand Brand { get; set; }

        /// <summary>
        /// 商品条形码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 商品有效期。单位:天。自发布/上架时间起计算，如果过期，则自动取消发布/下架。发布时，计算下架时间。
        /// 可提供功能，到期/即将到期时自动发布/上架用以重新计算上架/下架时间。
        /// </summary>
        public int? ValidThru { get; set; }


        ///// <summary>
        ///// 默认仓库
        ///// </summary>
        //public int? DefaultWarehouseId { get; set; }

        //public Warehouse DefaultWarehouse { get; set; }

        //public int StockId { get; set; }

        //public Stock Stock { get; set; }


        /// <summary>
        /// Gets or sets the order minimum quantity
        /// </summary>
        public int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public int OrderMaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock availability
        /// </summary>
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock quantity
        /// </summary>
        public bool DisplayStockQuantity { get; set; }

        /// <summary>
        /// 库存扣减策略，总共有2种：下单减库存(place_order_withhold)和支付减库存(payment_success_deduct)。
        /// </summary>
        public StockReduceStrategy StockReduceStrategy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this product is returnable (a customer is allowed to submit return request with this product)
        /// </summary>
        public bool NotReturnable { get; set; }

        public PublishType PublishType { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishedOn { get; set; }

        public DateTime? UnpublishedOn { get; set; }

        /// <summary>
        /// 取消发布原因
        /// </summary>
        public string UnpublishedReason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        public bool IsShipEnabled { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets the additional shipping charge
        /// </summary>
        public decimal AdditionalShippingCharge { get; set; }

        /// <summary>
        /// 运费模版Id
        /// </summary>
        public int? FreightTemplateId { get; set; }

        /// <summary>
        /// 运费模板
        /// </summary>
        public FreightTemplate FreightTemplate { get; set; }

        public int? UnitId { get; set; }

        public Unit Unit { get; set; }

        /// <summary>
        /// 管理员备注
        /// </summary>
        public string AdminRemark { get; set; }

        /// <summary>
        /// 备货期。取值范围:1-60;单位:天。
        /// </summary>
        public int? DeliveryTime { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IList<ProductMedia> Medias { get; protected set; } = new List<ProductMedia>();

        public IList<ProductAttributeValue> AttributeValues { get; protected set; } = new List<ProductAttributeValue>();

        public IList<ProductOptionValue> OptionValues { get; protected set; } = new List<ProductOptionValue>();

        public IList<ProductCategory> Categories { get; protected set; } = new List<ProductCategory>();

        public IList<ProductPriceHistory> PriceHistories { get; protected set; } = new List<ProductPriceHistory>();

        public IList<ProductOptionCombination> OptionCombinations { get; protected set; } = new List<ProductOptionCombination>();

        //public IList<StockHistory> StockHistories { get; protected set; } = new List<StockHistory>();

        //public void AddStockHistory(StockHistory stockHistory)
        //{
        //    stockHistory.Product = this;
        //    StockHistories.Add(stockHistory);
        //}

        public void AddCategory(ProductCategory category)
        {
            category.Product = this;
            Categories.Add(category);
        }

        public void AddMedia(ProductMedia media)
        {
            media.Product = this;
            Medias.Add(media);
        }

        public void AddAttributeValue(ProductAttributeValue attributeValue)
        {
            attributeValue.Product = this;
            AttributeValues.Add(attributeValue);
        }

        public void AddOptionValue(ProductOptionValue optionValue)
        {
            optionValue.Product = this;
            OptionValues.Add(optionValue);
        }

        public void AddOptionCombination(ProductOptionCombination combination)
        {
            combination.Product = this;
            OptionCombinations.Add(combination);
        }

        public void AddPriceHistory(User loginUser)
        {
            var priceHistory = new ProductPriceHistory
            {
                CreatedBy = loginUser,
                UpdatedBy = loginUser,
                Product = this,
                Price = Price,
                OldPrice = OldPrice,
                SpecialPrice = SpecialPrice,
                SpecialPriceStart = SpecialPriceStart,
                SpecialPriceEnd = SpecialPriceEnd
            };
            PriceHistories.Add(priceHistory);
        }

        public Product Clone()
        {
            var product = new Product();
            product.Name = Name;
            product.MetaTitle = MetaTitle;
            product.MetaKeywords = MetaKeywords;
            product.MetaDescription = MetaDescription;
            product.ShortDescription = ShortDescription;
            product.Description = Description;
            product.Specification = Specification;
            product.Price = Price;
            product.OldPrice = OldPrice;
            product.SpecialPrice = SpecialPrice;
            product.SpecialPriceStart = SpecialPriceStart;
            product.SpecialPriceEnd = SpecialPriceEnd;
            product.HasOptions = HasOptions;
            product.IsVisibleIndividually = IsVisibleIndividually;
            product.IsFeatured = IsFeatured;
            product.IsAllowToOrder = IsAllowToOrder;
            product.IsCallForPricing = IsCallForPricing;
            product.BrandId = BrandId;
            product.StockTrackingIsEnabled = StockTrackingIsEnabled;
            product.Sku = Sku;
            product.Gtin = Gtin;
            product.NormalizedName = NormalizedName;
            product.DisplayOrder = DisplayOrder;
            product.Slug = Slug;

            product.IsPublished = IsPublished;
            product.PublishedOn = PublishedOn;
            product.Barcode = Barcode;
            product.DeliveryTime = DeliveryTime;
            product.ValidThru = ValidThru;
            //product.DefaultWarehouseId = DefaultWarehouseId;
            product.PublishType = PublishType;
            product.OrderMaximumQuantity = OrderMaximumQuantity;
            product.OrderMinimumQuantity = OrderMinimumQuantity;
            product.DisplayStockAvailability = DisplayStockAvailability;
            product.DisplayStockQuantity = DisplayStockQuantity;
            product.NotReturnable = NotReturnable;
            product.StockReduceStrategy = StockReduceStrategy;
            product.UnpublishedOn = UnpublishedOn;
            product.UnpublishedReason = UnpublishedReason;

            product.AdditionalShippingCharge = AdditionalShippingCharge;
            product.AdminRemark = AdminRemark;
            product.FreightTemplateId = FreightTemplateId;
            product.Height = Height;
            product.IsFreeShipping = IsFreeShipping;
            product.IsShipEnabled = IsShipEnabled;
            product.Length = Length;
            product.Weight = Weight;
            product.Width = Width;
            product.UnitId = UnitId;

            //不赋值字段
            //IsVisibleIndividually

            foreach (var attribute in AttributeValues)
            {
                product.AddAttributeValue(new ProductAttributeValue
                {
                    AttributeId = attribute.AttributeId,
                    Value = attribute.Value
                });
            }

            foreach (var category in Categories)
            {
                product.AddCategory(new ProductCategory
                {
                    CategoryId = category.CategoryId
                });
            }

            return product;
        }
    }
}
