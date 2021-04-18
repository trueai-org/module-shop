namespace Shop.Module.Orders.Abstractions.ViewModels
{
    public class OrderCreateResult
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
