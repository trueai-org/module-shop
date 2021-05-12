using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductAttributeTemplateResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<ProductAttributeResult> Attributes { get; set; } = new List<ProductAttributeResult>();

        public IList<int> AttributesIds => this.Attributes.Select(c => c.Id).ToArray();
    }
}
