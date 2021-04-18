using System;

namespace Shop.Module.Shipments.Abstractions.ViewModels
{
    public class ShipmentQueryParam
    {
        public long? OrderNo { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime? ShippedOnStart { get; set; }

        public DateTime? ShippedOnEnd { get; set; }
    }
}
