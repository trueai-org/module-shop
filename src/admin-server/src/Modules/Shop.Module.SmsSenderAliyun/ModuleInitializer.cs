using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.SmsSenderAliyun.Services;

namespace Shop.Module.SmsSenderAliyun
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISmsSender, AliyunSmsSenderService>();
        }
    }
}
