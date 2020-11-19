using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Abstractions.Cache;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Extensions;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Services;

namespace Shop.Module.Core
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var json = configuration.GetValue<string>(nameof(ShopConfig));
            var config = new ShopConfig();
            if (!string.IsNullOrWhiteSpace(json))
                config = JsonConvert.DeserializeObject<ShopConfig>(json) ?? new ShopConfig();
            //configuration.GetSection("Shop").Bind(config);
            services.AddSingleton(config);

            var authConfig = new AuthenticationConfig();
            configuration.GetSection("Authentication").Bind(authConfig);
            services.AddSingleton(authConfig);

            services.AddTransient<IEntityService, EntityService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IAppSettingService, AppSettingService>();
            services.AddTransient<IWidgetInstanceService, WidgetInstanceService>();
            services.AddTransient<IUserAddressService, UserAddressService>();

            services.AddScoped<IWorkContext, WorkContext>();

            //serviceCollection.AddTransient<IThemeService, ThemeService>();
            //serviceCollection.AddTransient<IWidgetInstanceService, WidgetInstanceService>();
            //serviceCollection.AddScoped<SignInManager<User>, SimplSignInManager<User>>();
            //serviceCollection.AddSingleton<SettingDefinitionProvider>();
            //serviceCollection.AddScoped<ISettingService, SettingService>();

            services.AddScoped<SignInManager<User>>();
            //services.AddScoped<ShopSignInManager<User>>();
            //services.AddScoped<SignInManager<User>, ShopSignInManager<User>>();

            // used redis
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddHttpContextAccessor();

            // LocalCache is registered as transient as its implementation resolves IMemoryCache, thus
            // there is no state to keep in its instance.
            //services.AddTransient<IDistributedCache, MemoryDistributedCache>();
            services.AddDistributedMemoryCache();

            //services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddMemoryCache();

            services.AddScoped<ICacheManager, PerRequestCacheManager>();

            //register dependencies
            var shopConfig = services.BuildServiceProvider().GetService<ShopConfig>();
            if (shopConfig.RedisCachingEnabled)
            {
                services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>()
                    .AddSingleton<ILocker, RedisConnectionWrapper>();
                services.AddScoped<IStaticCacheManager, RedisCacheManager>();
            }
            else
            {
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>()
                    .AddSingleton<ILocker, MemoryCacheManager>();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
