using System.ComponentModel;

namespace Shop.Module.Orders.Models
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 新订单
        /// </summary>
        [Description("新订单")]
        New = 0,
        /// <summary>
        /// 挂起
        /// 冻结中的订单、纠纷中的订单、退款中的订单
        /// </summary>
        [Description("挂起")]
        OnHold = 10,
        /// <summary>
        /// 等待付款/待付款
        /// </summary>
        [Description("待付款")]
        PendingPayment = 20,
        /// <summary>
        /// 付款失败
        /// 允许重新付款，等同于待付款
        /// </summary>
        [Description("付款失败")]
        PaymentFailed = 25,
        /// <summary>
        /// 收到付款/已付款
        /// </summary>
        [Description("已付款")]
        PaymentReceived = 30,
        /// <summary>
        /// 发货处理中
        /// 运输状态：待发货、部分发货
        /// </summary>
        [Description("发货中")]
        Shipping = 40,
        /// <summary>
        /// 已发货
        /// 运输状态：无航运、已发货
        /// </summary>
        [Description("已发货")]
        Shipped = 50,
        /// <summary>
        /// 完成/交易成功
        /// 运输状态：确认收货
        /// </summary>
        [Description("交易成功")]
        Complete = 60,
        /// <summary>
        /// 取消/关闭
        /// 买家、卖家家申请取消订单；到期未付款交易自动取消
        /// 付款以后用户退款成功，交易自动关闭/取消
        /// </summary>
        [Description("交易取消")]
        Canceled = 70
    }
}