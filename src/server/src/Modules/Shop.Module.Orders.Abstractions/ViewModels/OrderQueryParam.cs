using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderQueryParam
    {
        public int? CustomerId { get; set; }
        public IList<OrderStatus> OrderStatus { get; set; } = new List<OrderStatus>();
        public IList<ShippingStatus> ShippingStatus { get; set; } = new List<ShippingStatus>();
        public DateTime? CreatedOnStart { get; set; }
        public DateTime? CreatedOnEnd { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
    }
}
