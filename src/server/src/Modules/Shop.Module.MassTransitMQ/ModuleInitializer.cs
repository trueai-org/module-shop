using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shop.Infrastructure.Modules;
using Shop.Module.MassTransitMQ.Services;
using Shop.Module.MQ;

namespace Shop.Module.MassTransitMQ
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var options = new RabbitMQOptions();
            var section = configuration.GetSection(nameof(RabbitMQOptions));
            section.Bind(options);
            services.Configure<RabbitMQOptions>(section);

            void configure(IBusRegistrationContext context, IBusFactoryConfigurator cfg)
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
            }

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductViewMQConsumer>();
                x.AddConsumer<ReplyAutoApprovedMQConsumer>();
                x.AddConsumer<ReviewAutoApprovedMQConsumer>();
                x.AddConsumer<PaymentReceivedMQConsumer>();

                if (options.Enabled)
                {
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(options.Host, options.Port, "/", h =>
                        {
                            h.Username(options.Username);
                            h.Password(options.Password);
                        });

                        configure(context, cfg);
                        cfg.ConfigureEndpoints(context);
                    });
                }
                else
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        configure(context, cfg);
                        cfg.ConfigureEndpoints(context);
                    });
                }
            });

            if (options.Enabled)
            {
                services.TryAddSingleton<IMQService, RabbitMQService>();
            }
            else
            {
                services.TryAddSingleton<IMQService, MemoryMQService>();
            }

            services.AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
