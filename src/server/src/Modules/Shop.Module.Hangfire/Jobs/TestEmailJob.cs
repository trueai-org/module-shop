using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Hangfire.Jobs
{
    public class TestEmailJob : BackgroundService
    {
        private readonly ILogger<TestEmailJob> _logger;
        public TestEmailJob(ILogger<TestEmailJob> logger)
        {
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                RecurringJob.AddOrUpdate("email_send", () => EmailSend(), Cron.MinuteInterval(2)); //2分钟一次 //Cron.Daily(1, 10) //每天9:10
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server monitoring, email job error");
            }
            return Task.CompletedTask;
        }

        public void EmailSend()
        {
            _logger.LogInformation("email send do worker.");
        }
    }
}
