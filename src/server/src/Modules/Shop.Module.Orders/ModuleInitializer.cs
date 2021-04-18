using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Orders.Abstractions.Events;
using Shop.Module.Orders.Abstractions.Services;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Services;

namespace Shop.Module.Orders
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IOrderService, OrderService>();

            //services.AddTransient<IOrderEmailService, OrderEmailService>();
            //services.AddHostedService<OrderCancellationBackgroundService>();

            services.AddTransient<INotificationHandler<OrderChanged>, OrderChangedCreateOrderHistoryHandler>();
            services.AddTransient<INotificationHandler<OrderCreated>, OrderCreatedCreateOrderHistoryHandler>();
            services.AddTransient<INotificationHandler<PaymentReceived>, PaymentReceivedHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
