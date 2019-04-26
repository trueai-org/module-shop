using System.Collections.Generic;

namespace Shop.Module.Catalog.Abstractions.ViewModels
{
    public class ProductCreateOptionParam
    {
        public int Id { get; set; }

        public IList<ProductCreateOptionValueParam> Values { get; set; } = new List<ProductCreateOptionValueParam>();
    }
}
