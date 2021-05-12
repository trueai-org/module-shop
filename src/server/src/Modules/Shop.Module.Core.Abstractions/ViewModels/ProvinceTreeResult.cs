using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public class ProvinceTreeResult
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public IList<ProvinceTreeResult> Children { get; set; } = new List<ProvinceTreeResult>();
    }
}
