using Shop.Module.Orders.Models;
using System.Collections.Generic;

namespace Shop.Module.Orders.ViewModels
{
    public class CustomerOrderQueryParam
    {
        public IList<OrderStatus> OrderStatus { get; set; } = new List<OrderStatus>();

        public ShippingStatus? ShippingStatus { get; set; }
    }
}