using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Shop.Module.Catalog.ViewModels
{
    public class WidgetProductSetting
    {
        public int ItemCount { get; set; }

        public int? CategoryId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WidgetProductOrderBy OrderBy { get; set; }

        public bool FeaturedOnly { get; set; }
    }
}
