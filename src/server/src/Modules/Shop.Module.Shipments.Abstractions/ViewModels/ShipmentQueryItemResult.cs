using System;

namespace Shop.Module.Shipments.ViewModels
{
    public class ShipmentQueryItemResult
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        /// <summary>
        /// 产品名称（快照）
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品图片（快照）
        /// </summary>
        public string ProductMediaUrl { get; set; }

        /// <summary>
        /// 下单数量
        /// </summary>
        public int OrderedQuantity { get; set; }

        /// <summary>
        /// 已发货数量
        /// </summary>
        public int ShippedQuantity { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
