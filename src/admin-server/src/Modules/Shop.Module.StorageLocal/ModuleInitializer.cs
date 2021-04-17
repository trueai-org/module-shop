using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Abstractions.Services;

namespace Shop.Module.StorageLocal
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            //由于ShopDbContext，因此不能使用单例
            //serviceCollection.AddSingleton<IStorageService, LocalStorageService>();
            serviceCollection.AddScoped<IStorageService, LocalStorageService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
