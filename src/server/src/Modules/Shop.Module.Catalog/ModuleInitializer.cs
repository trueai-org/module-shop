using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Catalog.Abstractions.Services;
using Shop.Module.Catalog.Handlers;
using Shop.Module.Catalog.Services;
using Shop.Module.Core.Abstractions.Events;

namespace Shop.Module.Catalog
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICategoryService, CategoryService>();
            serviceCollection.AddTransient<IBrandService, BrandService>();
            serviceCollection.AddTransient<IProductPricingService, ProductPricingService>();
            serviceCollection.AddTransient<IProductService, ProductService>();
            serviceCollection.AddTransient<INotificationHandler<EntityViewed>, EntityViewedHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
