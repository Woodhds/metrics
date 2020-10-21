using System;
using System.Linq.Expressions;
using Hangfire;
using metrics.BackgroundJobs.Abstractions;

namespace metrics.BackgroundJobs
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public BackgroundJobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        public string Execute<TService>(Expression<Action<TService>> action)
        {
            return _backgroundJobClient.Enqueue(action);
        }

        public string Schedule<TService>(Expression<Action<TService>> task, DateTimeOffset date)
        {
            return _backgroundJobClient.Schedule(task, date);
        }

        public string Schedule<TService>(Expression<Action<TService>> action, TimeSpan delay)
        {
            return _backgroundJobClient.Schedule(action, delay);
        }

        public void Register<TService>(string jobId, Expression<Action<TService>> action, string recurring)
        {
            _recurringJobManager.AddOrUpdate(jobId, action, recurring);
        }
    }
}