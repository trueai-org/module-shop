using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.MQ.Abstractions.Services;
using Shop.Module.RabbitMQ.Services;

namespace Shop.Module.RabbitMQ
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddTransient<IMQService, MQService>();
            // 实例化太耗时间，暂时使用单例 TODO
            services.AddSingleton<IMQService, MQService>();

            services.AddHostedService<ProductViewMQBackgroundService>();
            services.AddHostedService<ReplyAutoApprovedMQBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
