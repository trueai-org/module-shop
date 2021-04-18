using Shop.Module.Orders.Abstractions.Models;
using System.Collections.Generic;

namespace Shop.Module.Orders.Abstractions.ViewModels
{
    public class CustomerOrderQueryParam
    {
        public IList<OrderStatus> OrderStatus { get; set; } = new List<OrderStatus>();

        public ShippingStatus? ShippingStatus { get; set; }
    }
}