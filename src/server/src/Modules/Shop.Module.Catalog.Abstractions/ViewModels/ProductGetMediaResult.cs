namespace Shop.Module.Catalog.ViewModels
{
    public class ProductGetMediaResult
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Caption { get; set; }

        public int MediaId { get; set; }

        public string MediaUrl { get; set; }

        public int DisplayOrder { get; set; }
    }
}
