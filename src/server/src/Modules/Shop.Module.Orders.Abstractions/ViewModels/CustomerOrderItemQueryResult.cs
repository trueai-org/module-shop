namespace Shop.Module.Orders.ViewModels
{
    public class CustomerOrderItemQueryResult
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductMediaUrl { get; set; }

        /// <summary>
        /// 产品价格（原价）
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 折扣小计（折扣总金额）
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 金额小计（产品价格*数量 - 折扣总金额）
        /// </summary>
        public decimal ItemAmount { get; set; }

        /// <summary>
        /// 重量小计
        /// </summary>
        public decimal ItemWeight { get; set; }

        /// <summary>
        /// 备注、折扣备注
        /// 例如：满2件8折；满3件6折；活动价90；6折；活动1xxx，活动2xxx
        /// 活动信息
        /// </summary>
        public string Note { get; set; }

        public int ShippedQuantity { get; set; }

        /// <summary>
        /// 是否已评论
        /// </summary>
        public bool IsReviewed { get; set; }
    }
}
