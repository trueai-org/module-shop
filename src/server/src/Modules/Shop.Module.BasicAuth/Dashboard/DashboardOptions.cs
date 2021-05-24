using System.Collections.Generic;

namespace Shop.Module.BasicAuth.Dashboard
{
    public class DashboardOptions
    {
        public DashboardOptions()
        {
            Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() };
        }

        public IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }

        public bool IgnoreAntiforgeryToken { get; set; }
    }
}
