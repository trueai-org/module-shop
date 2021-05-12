using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.Orders.ViewModels
{
    public class CheckoutItemResult
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductPriceString => ProductPrice.ToString("C");

        public int Quantity { get; set; }

        public decimal Total => Quantity * ProductPrice;

        public string TotalString => Total.ToString("C");

        public IEnumerable<ProductVariationOption> VariationOptions { get; set; } = new List<ProductVariationOption>();

        public string VariationString { get { return string.Join(" ", VariationOptions.OrderBy(c => c.OptionName).Select(c => c.Value)); } }

        public bool IsPublished { get; set; }

        public int StockQuantity { get; set; }

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
    }
}
