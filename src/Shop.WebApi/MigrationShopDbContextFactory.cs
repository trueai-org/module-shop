using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure;
using Shop.Module.Core.Data;
using Shop.WebApi.Extensions;
using System;
using System.IO;

namespace Shop.WebApi
{
    public class MigrationShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var contentRootPath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                            .SetBasePath(contentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environmentName}.json", true);


            builder.AddUserSecrets(typeof(MigrationShopDbContextFactory).Assembly, optional: true);
            builder.AddEnvironmentVariables();
            var _configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();
            GlobalConfiguration.ContentRootPath = contentRootPath;
            GlobalConfiguration.Configuration = _configuration;

            services.AddModules(contentRootPath);
            services.AddCustomizedDataStore(_configuration);
            var _serviceProvider = services.BuildServiceProvider();

            return _serviceProvider.GetRequiredService<ShopDbContext>();
        }
    }
}
