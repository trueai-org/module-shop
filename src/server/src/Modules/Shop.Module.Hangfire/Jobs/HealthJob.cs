using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Hangfire.Jobs
{
    public class HealthJob : BackgroundService
    {
        private readonly ILogger _logger;
        public HealthJob(ILogger<HealthJob> logger)
        {
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                RecurringJob.AddOrUpdate("HealthJob", () => _logger.LogInformation($"Health Job is running. {DateTime.Now:yyyy:MM:dd HH:mm:ss.fff}"), Cron.Minutely());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server monitoring, Health Job Error");
            }
            return Task.CompletedTask;
        }
    }
}
