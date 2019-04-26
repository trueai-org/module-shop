using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            //cfg.ReceiveEndpoint(QueueKeys.ProductView, e =>
            //{
            //    e.Consumer<ProductViewMQConsumer>();
            //});
            //cfg.ReceiveEndpoint(queue, e =>
            //{
            //    e.Handler<T>(async context =>
            //    {
            //        await Task.Run(() =>
            //        {
            //            callback(context.Message);
            //        });
            //    });
            //});
            //cfg.ReceiveEndpoint(QueueKeys.ProductView, endpoint =>
            // {
            //     endpoint.Handler<ProductViewedMessage>(async context =>
            //     {
            //         await Console.Out.WriteLineAsync($"Received: {context.Message}");
            //     });
            // });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductViewMQConsumer>();
                x.AddConsumer<ReplyAutoApprovedMQConsumer>();
                x.AddConsumer<ReviewAutoApprovedMQConsumer>();
                x.AddConsumer<PaymentReceivedMQConsumer>();

                x.AddBus(p => Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.ReceiveEndpoint(QueueKeys.ProductView, e =>
                    {
                        e.ConfigureConsumer<ProductViewMQConsumer>(p);
                    });

                    cfg.ReceiveEndpoint(QueueKeys.ReplyAutoApproved, e =>
                    {
                        e.ConfigureConsumer<ReplyAutoApprovedMQConsumer>(p);
                    });

                    cfg.ReceiveEndpoint(QueueKeys.ReviewAutoApproved, e =>
                    {
                        e.ConfigureConsumer<ReviewAutoApprovedMQConsumer>(p);
                    });

                    cfg.ReceiveEndpoint(QueueKeys.PaymentReceived, e =>
                    {
                        e.ConfigureConsumer<PaymentReceivedMQConsumer>(p);
                    });

                    cfg.ConfigureEndpoints(p);
                }));
            });

            services.AddSingleton<IMQService, MQService>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQHostedService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

        }
    }
}
