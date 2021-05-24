using Microsoft.AspNetCore.Builder;
using System;

namespace Shop.Module.ApiProfiler
{
    /// <summary>
    /// Extension methods for the MiniProfiler middleware.
    /// </summary>
    public static class MiniProfilerBuilderExtensions
    {
        /// <summary>
        /// Adds middleware for profiling HTTP requests.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
        public static void UseCsaApiProfiler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<MiniProfilerMiddleware>();
        }
    }
}
