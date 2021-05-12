using Hangfire;
using Microsoft.Extensions.Logging;
using Shop.Module.Schedule;
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
        public Task<string> Enqueue(Expression<Func<Task>> methodCall)
        {
            return Task.FromResult(BackgroundJob.Enqueue(methodCall));
        }

        /// <summary>
        /// Delayed jobs
        /// Delayed jobs are executed only once too, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public Task<string> Schedule(Expression<Func<Task>> methodCall, TimeSpan timeSpan)
        {
            return Task.FromResult(BackgroundJob.Schedule(methodCall, timeSpan));
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
            return Task.CompletedTask;
        }
    }
}



