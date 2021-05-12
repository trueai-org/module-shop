using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shop.Module.Schedule
{
    public interface IJobService
    {
        /// <summary>
        /// Fire-and-forget jobs
        /// Fire-and-forget jobs are executed only once and almost immediately after creation.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <returns></returns>
        Task<string> Enqueue(Expression<Func<Task>> methodCall);

        /// <summary>
        /// Delayed jobs
        /// Delayed jobs are executed only once too, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        Task<string> Schedule(Expression<Func<Task>> methodCall, TimeSpan timeSpan);

        /// <summary>
        /// Recurring jobs
        /// Recurring jobs fire many times on the specified CRON schedule.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="cronExpression">Cron.Daily</param>
        /// <returns></returns>
        Task AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression);
    }
}