using Microsoft.AspNetCore.Http;
using System;

namespace Shop.Module.BasicAuth.Dashboard
{
    public static class AspNetCoreDashboardContextExtensions
    {
        public static HttpContext GetHttpContext(this DashboardContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var aspNetCoreContext = context as AspNetCoreDashboardContext;
            if (aspNetCoreContext == null)
            {
                throw new ArgumentException($"Context argument should be of type `{nameof(AspNetCoreDashboardContext)}`!", nameof(context));
            }

            return aspNetCoreContext.HttpContext;
        }
    }
}