using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public class WidgetCarouselParam : WidgetParamBase
    {
        public IList<WidgetCarouselItem> Items = new List<WidgetCarouselItem>();
    }
}
