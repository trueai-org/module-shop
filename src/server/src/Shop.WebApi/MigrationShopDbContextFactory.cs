using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Module.Core.Data;
using Shop.WebApi.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Shop.WebApi
{
    public class MigrationShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var contentRootPath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Modules.json", true, true)
                .AddJsonFile($"appsettings.RateLimiting.json", true)
                .AddJsonFile($"appsettings.{environmentName}.json", true);

            builder.AddUserSecrets(typeof(MigrationShopDbContextFactory).Assembly, optional: true);
            builder.AddEnvironmentVariables();

            var configuration = builder.Build();

            GlobalConfiguration.ContentRootPath = contentRootPath;
            GlobalConfiguration.Configuration = configuration;

            IServiceCollection services = new ServiceCollection();
            services.AddModules(configuration);
            services.AddCustomizedDataStore(configuration);

            return services.BuildServiceProvider().GetRequiredService<ShopDbContext>();
        }
    }
}
