using System;
using System.Linq.Expressions;
using Hangfire;
using metrics.BackgroundJobs.Abstractions;

namespace metrics.BackgroundJobs
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public BackgroundJobService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public string Execute<TService>(Expression<Action<TService>> action)
        {
            return _backgroundJobClient.Enqueue(action);
        }

        public string Schedule<TService>(Expression<Action<TService>> task, DateTime date)
        {
            return _backgroundJobClient.Schedule(task, date);
        }

        public string Schedule<TService>(Expression<Action<TService>> action, TimeSpan delay)
        {
            return _backgroundJobClient.Schedule(action, delay);
        }
    }
}