using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Shop.Infrastructure;
using Shop.Module.Core.Extensions;
using Shop.WebApi.Extensions;
using System;

namespace Shop.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).ConfigureAppConfiguration((builderContext, config) =>
            {
                var env = builderContext.HostingEnvironment;
                var configuration = config.AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile("appsettings.Modules.json", true, true)
                .AddJsonFile($"appsettings.RateLimiting.json", true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .Build();

                var apolloEnabled = configuration.GetSection("ApolloEnabled").Get<bool>();
                if (apolloEnabled == true)
                {
                    config.AddApollo(configuration.GetSection("Apollo")).AddDefault();
                    if (env.IsDevelopment())
                    {
                        Com.Ctrip.Framework.Apollo.Logging.LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
                    }
                }

                config.AddEntityFrameworkConfig(opt => opt.UseCustomizedDataStore(configuration));

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            }).ConfigureLogging((loggingBuilder) =>
            {
                loggingBuilder.AddSerilog();
            }).UseSerilog((builderContext, config) =>
            {
                var env = builderContext.HostingEnvironment;
                if (env.IsDevelopment())
                {
                    config.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console();
                    SelfLog.Enable(Console.Error);
                }
            });
    }
}
