using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Shipments.ViewModels
{
    public class ShipmentQueryResult
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string OrderNo { get; set; }

        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 运输状态
        /// </summary>
        public ShippingStatus? ShippingStatus { get; set; }

        public string TrackingNumber { get; set; }

        public decimal TotalWeight { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippedOn { get; set; }

        /// <summary>
        /// 收货时间
        /// </summary>
        public DateTime? DeliveredOn { get; set; }

        public string AdminComment { get; set; }

        public string CreatedBy { get; set; }

        public IList<ShipmentQueryItemResult> Items { get; set; } = new List<ShipmentQueryItemResult>();
    }
}
