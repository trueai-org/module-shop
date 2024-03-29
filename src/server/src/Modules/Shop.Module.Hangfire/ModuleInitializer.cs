using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Hangfire.Redis;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shop.Infrastructure.Modules;
using Shop.Module.Hangfire.Jobs;
using Shop.Module.Hangfire.Models;
using Shop.Module.Hangfire.Services;
using Shop.Module.Schedule;
using StackExchange.Redis;

namespace Shop.Module.Hangfire
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var options = new HangfireOptions();
            var section = configuration.GetSection(nameof(HangfireOptions));
            section.Bind(options);
            services.Configure<HangfireOptions>(section);

            if (options.RedisEnabled)
            {
                var redisOptions = ConfigurationOptions.Parse(options.RedisConnection);
                RedisStorageOptions storageOptions = null;
                if (redisOptions.DefaultDatabase.HasValue)
                {
                    storageOptions ??= new RedisStorageOptions();
                    storageOptions.Db = redisOptions.DefaultDatabase.Value;
                }

                if (!string.IsNullOrWhiteSpace(redisOptions.ChannelPrefix))
                {
                    storageOptions ??= new RedisStorageOptions();
                    storageOptions.Prefix = redisOptions.ChannelPrefix;
                }

                services.AddHangfire(config =>
                {
                    config.UseRedisStorage(ConnectionMultiplexer.Connect(redisOptions), storageOptions);
                });
            }
            else
            {
                services.AddHangfire(config =>
                {
                    config.UseMemoryStorage();
                });
            }

            services.AddScoped<IJobService, JobService>();

            services.AddHostedService<HealthJob>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptionsMonitor<HangfireOptions>>();
            var config = options.CurrentValue;

            if (!string.IsNullOrWhiteSpace(config?.Username) && !string.IsNullOrWhiteSpace(config?.Password))
            {
                var hangfireOption = new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new[] { new BasicAuthAuthorizationUser { Login = config?.Username, PasswordClear = config?.Password } }
                };
                app.UseHangfireDashboard(options: new DashboardOptions
                {
                    Authorization = new[] { new BasicAuthAuthorizationFilter(hangfireOption) }
                });
            }
            else
            {
                app.UseHangfireDashboard();
            }
            app.UseHangfireServer();
        }
    }
}
