using System.Collections.Generic;

namespace Shop.Module.Orders.ViewModels
{
    public class CheckoutParam
    {
        public int? UserAddressId { get; set; }

        public int CustomerId { get; set; }

        public IList<CheckoutItemParam> Items { get; set; } = new List<CheckoutItemParam>();
    }
}