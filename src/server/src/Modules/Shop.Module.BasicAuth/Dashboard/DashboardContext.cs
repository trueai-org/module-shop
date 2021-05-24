using System;

namespace Shop.Module.BasicAuth.Dashboard
{
    public abstract class DashboardContext
    {
        protected DashboardContext( DashboardOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public DashboardOptions Options { get; }

        public DashboardRequest Request { get; protected set; }
        public DashboardResponse Response { get; protected set; }

        public string AntiforgeryHeader { get; set; }
        public string AntiforgeryToken { get; set; }
    }
}