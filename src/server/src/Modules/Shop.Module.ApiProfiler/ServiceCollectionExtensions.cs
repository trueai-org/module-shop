using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Shop.Module.ApiProfiler.Internal;
using Shop.Module.ApiProfiler.Storage;

namespace Shop.Module.ApiProfiler
{
    /// <summary>
    /// Extension methods for configuring MiniProfiler for MVC.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MiniProfiler timings for actions and views.
        /// </summary>
        /// <param name="services">The services collection to configure.</param>
        /// <param name="configureOptions">An Action{MiniProfilerOptions} to configure options for MiniProfiler.</param>
        public static void AddCsaApiProfiler(this IServiceCollection services, Action<MiniProfilerOptions> configureOptions = null)
        {
            services.AddSingleton<IConfigureOptions<MiniProfilerOptions>, MiniProfilerOptionsDefaults>();
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            // Set background statics
            services.Configure<MiniProfilerOptions>(o => MiniProfiler.Configure(o));

            // See https://github.com/MiniProfiler/dotnet/issues/162 for plans
            // Blocked on https://github.com/aspnet/Mvc/issues/6222
            services.AddTransient<IConfigureOptions<MvcOptions>, MiniProfilerSetup>();
        }
    }

    /// <summary>
    /// Configures the default (important: with DI for IMemoryCache) before further user configuration.
    /// </summary>
    internal class MiniProfilerOptionsDefaults : IConfigureOptions<MiniProfilerOptions>
    {
        private readonly IMemoryCache _cache;
        public MiniProfilerOptionsDefaults(IMemoryCache cache) => _cache = cache;

        public void Configure(MiniProfilerOptions options)
        {
            if (options.Storage == null)
            {
                options.Storage = new MemoryCacheStorage(_cache, TimeSpan.FromMinutes(30));
            }
        }
    }

    internal class MiniProfilerSetup : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            options.Filters.Add(new ProfilingActionFilter());
        }
    }
}
