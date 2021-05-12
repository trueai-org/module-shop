using Shop.Module.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.Orders.ViewModels
{
    public class CheckoutResult
    {
        public string CouponCode { get; set; }

        public string CouponValidationErrorMessage { get; set; }

        public decimal Discount { get; set; }

        public string DiscountString { get { return Discount.ToString("C"); } }

        public decimal? ShippingAmount { get; set; }

        public string ShippingAmountString { get { return ShippingAmount.HasValue ? ShippingAmount.Value.ToString("C") : "-"; } }

        public decimal SubTotal { get { return Items.Sum(x => x.Quantity * x.ProductPrice); } }

        public string SubTotalString { get { return SubTotal.ToString("C"); } }

        public decimal OrderTotal { get { return SubTotal + (ShippingAmount ?? 0) - Discount; } }

        public string OrderTotalString { get { return OrderTotal.ToString("C"); } }

        public int SubCount { get { return Items.Sum(x => x.Quantity); } }

        public string OrderNote { get; set; }

        public UserAddressShippingResult Address { get; set; }

        public IList<CheckoutItemResult> Items { get; set; } = new List<CheckoutItemResult>();
    }
}
