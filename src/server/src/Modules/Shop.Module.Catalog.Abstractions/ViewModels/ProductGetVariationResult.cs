using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductGetVariationResult
    {
        /// <summary>
        /// Product Id
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string Sku { get; set; }

        public string Gtin { get; set; }

        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public int? MediaId { get; set; }

        public string MediaUrl { get; set; }

        public int StockQuantity { get; set; }

        public IList<int> WarehouseIds { get; set; } = new List<int>();

        public IList<ProductGetOptionCombinationResult> OptionCombinations { get; set; } = new List<ProductGetOptionCombinationResult>();

        public IList<ProductGetStockResult> Stocks { get; set; } = new List<ProductGetStockResult>();
    }
}
