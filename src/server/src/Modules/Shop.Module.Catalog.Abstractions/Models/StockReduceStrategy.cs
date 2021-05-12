namespace Shop.Module.Catalog.Models
{
    /// <summary>
    /// 库存扣减策略，总共有2种：下单减库存(place_order_withhold)和支付减库存(payment_success_deduct)。
    /// </summary>
    public enum StockReduceStrategy
    {
        PlaceOrderWithhold = 0,
        PaymentSuccessDeduct = 1
    }
}
