using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class GoodsGetStockResult
    {
        public int StockQuantity { get; set; }

        public int ProductId { get; set; }

        public bool StockTrackingIsEnabled { get; set; }

        public bool IsAllowToOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock availability
        /// </summary>
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock quantity
        /// </summary>
        public bool DisplayStockQuantity { get; set; }

        public IList<ProductGetOptionCombinationResult> OptionCombinations { get; set; } = new List<ProductGetOptionCombinationResult>();
    }
}
