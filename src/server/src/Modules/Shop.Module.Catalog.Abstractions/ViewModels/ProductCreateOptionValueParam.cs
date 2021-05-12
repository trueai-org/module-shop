namespace Shop.Module.Catalog.ViewModels
{
    public class ProductCreateOptionValueParam
    {
        public string Value { get; set; }

        public string Display { get; set; }

        public int DisplayOrder { get; set; }

        public int? MediaId { get; set; }

        public bool IsDefault { get; set; }
    }
}
