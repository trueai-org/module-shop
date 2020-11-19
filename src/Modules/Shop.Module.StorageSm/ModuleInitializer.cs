using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.StorageSm.Services;

namespace Shop.Module.StorageSm
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddSingleton<IStorageService, SmStorageService>();
            serviceCollection.AddScoped<IStorageService, SmStorageService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
