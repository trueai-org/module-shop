using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public class HomeResult
    {
        public IList<HomeWidgetInstanceResult> WidgetInstances { get; set; } = new List<HomeWidgetInstanceResult>();
    }
}
