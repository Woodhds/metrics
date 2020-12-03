using System;
using System.Linq;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql.Contexts
{
    public class QueryContext : IQueryContext
    {
        private readonly DbContext _context;
        private bool _disposed;
        
        public QueryContext(DbContext context)
        {
            _context = context;
        }
        
        public IQueryable<T> Query<T>() where T : class
        {
            return _context.Set<T>();
        }

        public IQueryable<T> RawSql<T>(string sql, params object[] parameters) where T : class
        {
            return _context.Set<T>().FromSqlRaw(sql, parameters);
        }

        public IQueryable<T> RawSql<T>(FormattableString sql) where T : class
        {
            return _context.Set<T>().FromSqlInterpolated(sql);
        }

        ~QueryContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _context?.Dispose();
            }

            _disposed = true;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}