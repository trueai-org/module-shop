using Shop.Module.Catalog.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductGetResult
    {
        public int Id { get; set; }

        public int ParentGroupedProductId { get; set; }

        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public decimal? SpecialPrice { get; set; }

        public DateTime? SpecialPriceStart { get; set; }

        public DateTime? SpecialPriceEnd { get; set; }

        public bool IsCallForPricing { get; set; }

        public bool IsAllowToOrder { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string MetaTitle { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string Sku { get; set; }

        public string Gtin { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string Specification { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFeatured { get; set; }

        public bool StockTrackingIsEnabled { get; set; }

        public IList<ProductGetAttributeResult> Attributes { get; set; } = new List<ProductGetAttributeResult>();

        public IList<ProductGetOptionResult> Options { get; set; } = new List<ProductGetOptionResult>();

        public IList<ProductGetVariationResult> Variations { get; set; } = new List<ProductGetVariationResult>();

        public IList<ProductGetMediaResult> ProductImages { get; set; } = new List<ProductGetMediaResult>();

        public IList<ProductGetStockResult> Stocks { get; set; } = new List<ProductGetStockResult>();

        public IList<int> CategoryIds { get; set; } = new List<int>();

        public IList<int> MediaIds { get; set; } = new List<int>();

        public int? MediaId { get; set; }

        public string MediaUrl { get; set; }

        public int? BrandId { get; set; }

        public DateTime? PublishedOn { get; set; }

        /// <summary>
        /// 商品条形码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 商品有效期。单位:天。自发布/上架时间起计算，如果过期，则自动取消发布/下架。发布时，计算下架时间。
        /// 可提供功能，到期/即将到期时自动发布/上架用以重新计算上架/下架时间。
        /// </summary>
        public int? ValidThru { get; set; }

        /// <summary>
        /// 备货期。取值范围:1-60;单位:天。
        /// </summary>
        public int? DeliveryTime { get; set; }

        public IList<int> WarehouseIds { get; set; } = new List<int>();

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

        public DateTime? UnpublishedOn { get; set; }

        /// <summary>
        /// 取消发布原因
        /// </summary>
        public string UnpublishedReason { get; set; }

        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the values indicating whether this product is visible in catalog or search results.
        /// It's used when this product is associated to some "grouped" one
        /// This way associated products could be accessed/added/etc only from a grouped product details page
        /// </summary>
        public bool IsVisibleIndividually { get; set; }

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
        /// 管理员备注
        /// </summary>
        public string AdminRemark { get; set; }

        public int? UnitId { get; set; }
    }
}
