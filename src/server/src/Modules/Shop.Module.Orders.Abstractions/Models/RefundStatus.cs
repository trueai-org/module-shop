namespace Shop.Module.Orders.Models
{
    /// <summary>
    /// 退款状态
    /// </summary>
    public enum RefundStatus
    {
        /// <summary>
        /// 等待退款
        /// </summary>
        WaitRefund = 0,
        /// <summary>
        /// 退款成功
        /// </summary>
        RefundOk = 10,
        /// <summary>
        /// 退款取消
        /// </summary>
        RefundCancel = 20,
        /// <summary>
        /// 关闭
        /// </summary>
        Close = 30,
        /// <summary>
        /// 退款冻结
        /// </summary>
        RefundFrozen = 40
    }
}