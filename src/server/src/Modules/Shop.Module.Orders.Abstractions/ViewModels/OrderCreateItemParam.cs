namespace Shop.Module.Orders.ViewModels
{
    public class OrderCreateItemParam
    {
        /// <summary>
        /// ProductId
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 产品价格（原价）（允许修改）
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 数量（允许修改）
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 折扣小计（折扣总金额）（允许修改）
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 金额小计（产品价格*数量 - 折扣总金额）（允许修改）
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
    }
}
