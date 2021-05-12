namespace Shop.Module.Catalog.ViewModels
{
    public class ProductGetStockResult
    {
        public int Quantity { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsEnabled { get; set; }
    }
}
