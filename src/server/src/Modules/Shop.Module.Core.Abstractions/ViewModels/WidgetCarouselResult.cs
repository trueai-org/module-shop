using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public class WidgetCarouselResult : WidgetResultBase
    {
        public IList<WidgetCarouselItem> Items = new List<WidgetCarouselItem>();
    }
}
