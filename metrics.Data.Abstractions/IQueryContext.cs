using System;
using System.Linq;

namespace metrics.Data.Abstractions
{
    public interface IQueryContext : IDisposable
    {
        IQueryable<T> Query<T>() where T : class;
    }
}