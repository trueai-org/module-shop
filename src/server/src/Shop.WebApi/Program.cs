using AspNetCoreRateLimit;
using Com.Ctrip.Framework.Apollo.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Shop.Module.Core.Extensions;
using Shop.WebApi.Extensions;
using System;
using System.Threading.Tasks;

namespace Shop.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();

            // AspNetCoreRateLimit
            // https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/Version-3.0.0-Breaking-Changes
            using (var scope = webHost.Services.CreateScope())
            {
                // get the ClientPolicyStore instance
                var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

                // seed Client data from appsettings
                await clientPolicyStore.SeedAsync();

                // get the IpPolicyStore instance
                var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

                // seed IP data from appsettings
                await ipPolicyStore.SeedAsync();
            }

            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).ConfigureAppConfiguration((builderContext, config) =>
            {
                var env = builderContext.HostingEnvironment;
                var configuration = config
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.Modules.json", true, true)
                .AddJsonFile($"appsettings.RateLimiting.json", true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .Build();

                var apolloEnabled = configuration.GetSection("ApolloEnabled").Get<bool>();
                if (apolloEnabled == true)
                {
                    var apolloConfigurationBuilder = config.AddApollo(configuration.GetSection("Apollo"));

                    // 如果是开发环境，则本地配置项优先级最高
                    if (env.IsDevelopment())
                    {
                        LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);

                        apolloConfigurationBuilder
                        .AddJsonFile($"appsettings.json", true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
                    }

                    configuration = apolloConfigurationBuilder.Build();
                }

                config.AddEntityFrameworkConfig(opt => opt.UseCustomizedDataStore(configuration));

                var loggerConfig = new LoggerConfiguration();
                if (env.IsDevelopment())
                {
                  
                    loggerConfig.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console();
                    SelfLog.Enable(Console.Error);
                }

                Log.Logger = loggerConfig.ReadFrom.Configuration(configuration).CreateLogger();

            }).ConfigureLogging((loggingBuilder) =>
            {
                loggingBuilder.AddSerilog();
            });

        // FOR xxx
        //.UseSerilog((builderContext, config) =>
        //    {
        //        var env = builderContext.HostingEnvironment;
        //        if (env.IsDevelopment())
        //        {
        //            config.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console();
        //            SelfLog.Enable(Console.Error);
        //        }
        //    });
    }
}
