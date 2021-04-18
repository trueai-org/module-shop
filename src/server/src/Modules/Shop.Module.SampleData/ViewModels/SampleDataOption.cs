using System.Collections.Generic;

namespace Shop.Module.SampleData.ViewModels
{
    public class SampleDataOption
    {
        public string Industry { get; set; } = "Fashion";

        public IList<string> AvailableIndustries { get; set; } = new List<string>();
    }
}
