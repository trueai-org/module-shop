using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Reviews.Data;
using Shop.Module.Reviews.Events;
using Shop.Module.Reviews.Handlers;
using Shop.Module.Reviews.Repositories;
using Shop.Module.Reviews.Services;

namespace Shop.Module.Reviews
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
