using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Hangfire.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shop.Infrastructure.Modules;
using Shop.Module.Hangfire.Jobs;
using Shop.Module.Hangfire.Models;
using Shop.Module.Hangfire.Services;
using Shop.Module.Schedule.Abstractions.Services;
using StackExchange.Redis;

namespace Shop.Module.Hangfire
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider();
            var json = configuration.GetRequiredService<IConfiguration>().GetValue<string>(nameof(HangfireOptions));
            var hangfireConfig = JsonConvert.DeserializeObject<HangfireOptions>(json) ?? new HangfireOptions();

            //services.AddHangfire(config =>
            //{
            //    config.UseRedisStorage(ConnectionMultiplexer.Connect(hangfireConfig.RedisHangfireConnection), new RedisStorageOptions()
            //    {
            //        Db = 1,
            //        Prefix = "shop:"
            //    });
            //});

            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            //switch (hangfireConfig.Provider)
            //{
            //    case ProviderType.MySql:
            //        services.AddHangfire(config =>
            //        config.UseStorage(new MySqlStorage(hangfireConfig.MySqlHangfireConnection,
            //        new MySqlStorageOptions
            //        {
            //            TransactionIsolationLevel = IsolationLevel.ReadCommitted,
            //            QueuePollInterval = TimeSpan.FromSeconds(15),
            //            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            //            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            //            PrepareSchemaIfNecessary = true,
            //            DashboardJobListLimit = 50000,
            //            TransactionTimeout = TimeSpan.FromMinutes(1),
            //            TablesPrefix = "Hangfire" // TablesPrefix - prefix for the tables in database. Default is none
            //        })));
            //        break;
            //    case ProviderType.SqlServer:
            //        services.AddHangfire(config => config.UseSqlServerStorage(hangfireConfig.SqlServerHangfireConnection));
            //        break;
            //    case ProviderType.Redis:
            //        services.AddHangfire(config =>
            //        {
            //            config.UseRedisStorage(ConnectionMultiplexer.Connect(hangfireConfig.RedisHangfireConnection));
            //        });
            //        break;
            //    case ProviderType.Memory:
            //        services.AddHangfire(config =>
            //        {
            //            config.UseMemoryStorage();
            //        });
            //        break;
            //    default:
            //        services.AddHangfire(config =>
            //        {
            //            config.UseMemoryStorage();
            //        });
            //        break;
            //}

            //services.Configure<HangfireAuthOptions>(configuration.GetSection("Hangfire"));
            //services.Configure<HangfireOptions>(options =>
            //{
            //    options.Provider = hangfireConfig.Provider;
            //    options.MySqlHangfireConnection = hangfireConfig.MySqlHangfireConnection;
            //    options.RedisHangfireConnection = hangfireConfig.RedisHangfireConnection;
            //    options.SqlServerHangfireConnection = hangfireConfig.SqlServerHangfireConnection;
            //    options.Username = hangfireConfig.Username;
            //    options.Password = hangfireConfig.Password;
            //});
            services.AddScoped<IJobService, JobService>();
            services.AddHostedService<TestEmailJob>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //var option = app.ApplicationServices.GetRequiredService<IOptionsMonitor<HangfireOptions>>();
            var json = app.ApplicationServices.GetRequiredService<IConfiguration>().GetValue<string>(nameof(HangfireOptions));
            var hangfireConfig = JsonConvert.DeserializeObject<HangfireOptions>(json) ?? new HangfireOptions();

            var username = hangfireConfig.Username;
            var password = hangfireConfig.Password;
            if (hangfireConfig != null && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var hangfireOption = new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new[] { new BasicAuthAuthorizationUser { Login = username, PasswordClear = password } }
                };
                app.UseHangfireDashboard(options: new DashboardOptions
                {
                    Authorization = new[] { new BasicAuthAuthorizationFilter(hangfireOption) }
                });
                //app.UseHangfireDashboard("/hangfire", new DashboardOptions
                //{
                //    Authorization = new[] { new HangfireAuthorizationFilter() }
                //});
            }
            else
            {
                app.UseHangfireDashboard();
            }
            app.UseHangfireServer();
        }
    }
}
