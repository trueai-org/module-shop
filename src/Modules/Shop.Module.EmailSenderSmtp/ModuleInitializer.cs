using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Abstractions.Services;

namespace Shop.Module.EmailSenderSmtp
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmailSender, EmailSender>();
        }
    }
}
