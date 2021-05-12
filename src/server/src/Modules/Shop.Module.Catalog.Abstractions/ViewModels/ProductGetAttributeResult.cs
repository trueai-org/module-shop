using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductGetAttributeResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<ProductGetAttributeValueResult> Values { get; set; } = new List<ProductGetAttributeValueResult>();
    }
}
