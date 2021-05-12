using Shop.Module.Core.Data;

namespace Shop.Module.Orders.Data
{
    public class OrderKeys : ShopKeys
    {
        public static string Module = System + ":order";

        /// <summary>
        /// 客户创建订单锁
        /// </summary>
        public static string CustomerCreateOrderLock = Module + ":create:lock:";

        /// <summary>
        /// 订单下单后超时自动取消订单时间,默认2h(微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时)
        /// </summary>
        public const string OrderAutoCanceledTimeForMinute = "OrderAutoCanceledTimeForMinute";

        /// <summary>
        /// 订单支付后超时自动完成订单时间(买家未在指定的时间内确认收货,则系统自动确认收货完成订单),默认7d
        /// 允许买家延长确认收货时间
        /// </summary>
        public const string OrderAutoCompleteTimeForMinute = "OrderAutoCompleteTimeForMinute";

        /// <summary>
        /// 订单完成后超时自动好评时间(买家未在指定的时间内评价,则系统自动好评),默认7d
        /// </summary>
        public const string OrderCompleteAutoReviewTimeForMinute = "OrderCompleteAutoReviewTimeForMinute";
    }
}
