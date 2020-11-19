using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shop.Infrastructure.Modules;
using Shop.Module.MassTransitMQ.Services;
using Shop.Module.MQ.Abstractions.Data;
using Shop.Module.MQ.Abstractions.Services;

namespace Shop.Module.MassTransitMQ
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductViewMQConsumer>();
                x.AddConsumer<ReplyAutoApprovedMQConsumer>();
                x.AddConsumer<ReviewAutoApprovedMQConsumer>();
                x.AddConsumer<PaymentReceivedMQConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint(QueueKeys.ProductView, e =>
                    {
                        e.ConfigureConsumer<ProductViewMQConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueKeys.ReplyAutoApproved, e =>
                    {
                        e.ConfigureConsumer<ReplyAutoApprovedMQConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueKeys.ReviewAutoApproved, e =>
                    {
                        e.ConfigureConsumer<ReviewAutoApprovedMQConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueKeys.PaymentReceived, e =>
                    {
                        e.ConfigureConsumer<PaymentReceivedMQConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                //x.UsingRabbitMq((context, cfg) =>
                //{
                //});
            });

            services.TryAddSingleton<IMQService, MemoryMQService>();

            //services.TryAddSingleton<IMQService, RabbitMQService>();

            services.AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
