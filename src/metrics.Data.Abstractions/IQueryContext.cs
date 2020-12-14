using System;
using System.Linq;

namespace metrics.Data.Abstractions
{
    public interface IQueryContext : IDisposable, IAsyncDisposable
    {
        IQueryable<T>? Query<T>() where T : class;
        IQueryable<T>? RawSql<T>(string sql, params object[] parameters) where T : class;
        IQueryable<T>? RawSql<T>(FormattableString sql) where T : class;
    }
}