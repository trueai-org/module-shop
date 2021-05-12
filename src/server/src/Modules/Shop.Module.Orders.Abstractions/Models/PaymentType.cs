namespace Shop.Module.Orders.Models
{
    /// <summary>
    /// 付款类型
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// 在线支付
        /// </summary>
        OnlinePayment = 0,
        /// <summary>
        /// 货到付款
        /// </summary>
        CashOnDelivery = 1
    }
}
