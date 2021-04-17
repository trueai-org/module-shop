using Hangfire;
using Microsoft.Extensions.Logging;
using Shop.Module.Schedule.Abstractions.Services;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shop.Module.Hangfire.Services
{
    public class JobService : IJobService
    {
        private readonly ILogger<JobService> _logger;
        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Fire-and-forget jobs
        /// Fire-and-forget jobs are executed only once and almost immediately after creation.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <returns></returns>
        public Task Enqueue(Expression<Func<Task>> methodCall)
        {
            var jobId = BackgroundJob.Enqueue(methodCall);
            _logger.LogInformation("Enqueue jobId: " + jobId);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Delayed jobs
        /// Delayed jobs are executed only once too, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public Task Schedule(Expression<Func<Task>> methodCall, TimeSpan timeSpan)
        {
            var jobId = BackgroundJob.Schedule(methodCall, timeSpan);
            _logger.LogInformation("Schedule jobId: " + jobId);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Recurring jobs
        /// Recurring jobs fire many times on the specified CRON schedule.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="cronExpression">Cron.Daily</param>
        /// <returns></returns>
        public Task AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression)
        {
            // RecurringJob.AddOrUpdate("Server monitoring", () => EmailSend(), Cron.Daily(1, 10)); //每天9:10
            // 设置时区在多服务器运行时可能会报错
            RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
            return Task.FromResult(0);
        }
    }
}



