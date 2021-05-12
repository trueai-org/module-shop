using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderShipmentParam
    {
        [Required]
        public string TrackingNumber { get; set; }

        public decimal TotalWeight { get; set; }

        public string AdminComment { get; set; }

        public IList<OrderShipmentItemParam> Items { get; set; } = new List<OrderShipmentItemParam>();
    }
}
