using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class WidgetSimpleProductSetting
    {
        public IList<ProductLinkResult> Products { get; set; } = new List<ProductLinkResult>();
    }
}
