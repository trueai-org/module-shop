using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.ShoppingCart.ViewModels
{
    public class CartResult
    {
        public int Id { get; set; }

        public string CouponCode { get; set; }

        public string CouponValidationErrorMessage { get; set; }

        public decimal Discount { get; set; }

        public string DiscountString { get { return Discount.ToString("C"); } }

        public decimal? ShippingAmount { get; set; }

        public string ShippingAmountString { get { return ShippingAmount.HasValue ? ShippingAmount.Value.ToString("C") : "-"; } }

        public decimal SubTotal { get { return Items.Sum(x => x.Quantity * x.ProductPrice); } }

        public string SubTotalString { get { return SubTotal.ToString("C"); } }

        public decimal CheckedSubTotal { get { return Items.Where(x => x.IsChecked).Sum(x => x.Quantity * x.ProductPrice); } }

        public string CheckedSubTotalString { get { return CheckedSubTotal.ToString("C"); } }

        public decimal OrderTotal { get { return SubTotal + (ShippingAmount ?? 0) - Discount; } }

        public string OrderTotalString { get { return OrderTotal.ToString("C"); } }

        public decimal CheckedOrderTotal { get { return CheckedSubTotal + (ShippingAmount ?? 0) - Discount; } }

        public string CheckedOrderTotalString { get { return CheckedOrderTotal.ToString("C"); } }

        public int SubCount { get { return Items.Sum(x => x.Quantity); } }

        public int CheckedSubCount { get { return Items.Where(x => x.IsChecked).Sum(x => x.Quantity); } }

        public string OrderNote { get; set; }

        public IList<CartItemResult> Items { get; set; } = new List<CartItemResult>();
    }
}
