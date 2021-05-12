using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public class WidgetCarouselComponentResult
    {
        public int Id { get; set; }

        public int WidgetZoneId { get; set; }

        public int WidgetId { get; set; }

        public int DataInterval { get; set; } = 6000;

        public IList<WidgetCarouselItem> Items { get; set; } = new List<WidgetCarouselItem>();
    }
}
