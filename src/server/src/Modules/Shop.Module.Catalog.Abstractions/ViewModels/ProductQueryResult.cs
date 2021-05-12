using System;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductQueryResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool HasOptions { get; set; }

        public bool IsVisibleIndividually { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsCallForPricing { get; set; }

        public bool IsAllowToOrder { get; set; }

        public int? StockQuantity { get; set; }

        public decimal Price { get; set; }

        public string MediaUrl { get; set; }
    }
}
