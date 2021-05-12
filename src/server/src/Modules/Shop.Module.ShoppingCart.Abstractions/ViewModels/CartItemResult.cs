using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.ShoppingCart.ViewModels
{
    public class CartItemResult
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

        public bool IsChecked { get; set; }

        public IEnumerable<ProductVariationOption> VariationOptions { get; set; } = new List<ProductVariationOption>();

        public string VariationString { get { return string.Join(" ", VariationOptions.OrderBy(c => c.OptionName).Select(c => c.Value)); } }
    }
}
