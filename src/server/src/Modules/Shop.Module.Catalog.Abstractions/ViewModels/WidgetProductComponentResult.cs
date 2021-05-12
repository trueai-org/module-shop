using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class WidgetProductComponentResult
    {
        public int Id { get; set; }

        public int WidgetZoneId { get; set; }

        public int WidgetId { get; set; }

        public string WidgetName { get; set; }

        public WidgetProductSetting Setting { get; set; }

        public IList<GoodsListResult> Products { get; set; } = new List<GoodsListResult>();
    }
}
