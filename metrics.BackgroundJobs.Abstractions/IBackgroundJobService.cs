using System;
using System.Linq.Expressions;

namespace metrics.BackgroundJobs.Abstractions
{
    public interface IBackgroundJobService
    {
        string Execute<TService>(Expression<Action<TService>> action);
        string Schedule<TService>(Expression<Action<TService>> task, DateTimeOffset date);
        string Schedule<TService>(Expression<Action<TService>> action, TimeSpan delay);
    }
}