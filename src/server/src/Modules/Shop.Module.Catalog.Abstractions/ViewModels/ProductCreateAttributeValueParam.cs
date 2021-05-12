using System.Collections;
using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductCreateAttributeValueParam
    {
        public int AttributeId { get; set; }

        public IList<string> Values { get; set; } = new List<string>();
    }
}
