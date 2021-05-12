namespace Shop.Module.Orders.Data
{
    public class OrderOptions
    {
        /// <summary>
        /// 订单下单后超时自动取消订单时间,默认2h(微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时)
        /// </summary>
        public int OrderAutoCanceledTimeForMinute { get; set; } = 120;

        /// <summary>
        /// 订单支付后超时自动完成订单时间(买家未在指定的时间内确认收货,则系统自动确认收货完成订单),默认7d
        /// </summary>
        public int OrderAutoCompleteTimeForMinute { get; set; } = 10080;

        /// <summary>
        /// 订单完成后超时自动好评时间(买家未在指定的时间内评价,则系统自动好评),默认7d
        /// </summary>
        public int OrderCompleteAutoReviewTimeForMinute { get; set; } = 10080;
    }
}
