using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Reviews.Abstractions.Events;
using Shop.Module.Reviews.Abstractions.Repositories;
using Shop.Module.Reviews.Data;
using Shop.Module.Reviews.Handlers;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.Services.Abstractions;

namespace Shop.Module.Reviews
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IReviewRepository, ReviewRepository>();
            services.AddTransient<IReviewService, ReviewService>();

            services.AddTransient<INotificationHandler<ReplyAutoApprovedEvent>, ReplyAutoApprovedHandler>();
            services.AddTransient<INotificationHandler<ReviewAutoApprovedEvent>, ReviewAutoApprovedHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
