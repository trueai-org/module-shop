namespace Shop.Module.Orders.Models
{
    /// <summary>
    /// 运输状态
    /// </summary>
    public enum ShippingStatus
    {
        /// <summary>
        /// No Shipping
        /// 没有航运
        /// </summary>
        NoShipping = 0,

        /// <summary>
        /// Not yet shipped
        /// 尚未发货
        /// WAIT_SELLER_SEND_GOODS(等待卖家发货,即:买家已付款) 
        /// </summary>
        NotYetShipped = 20,

        /// <summary>
        /// Partially shipped
        /// 部分发货
        /// SELLER_PART_SEND_GOODS:部分发货;SELLER_CONSIGNED_PART(卖家部分发货)
        /// </summary>
        PartiallyShipped = 25,

        /// <summary>
        /// Shipped
        /// 已发货/全部已发货
        /// WAIT_BUYER_ACCEPT_GOODS:等待买家收货;WAIT_BUYER_CONFIRM_GOODS(等待买家确认收货,即:卖家已发货);卖家已发货 SELLER_SEND_GOODS 
        /// </summary>
        Shipped = 30,

        /// <summary>
        /// Delivered
        /// 买家已确认收货 BUYER_ACCEPT_GOODS,NO_LOGISTICS
        /// </summary>
        Delivered = 40
    }
}