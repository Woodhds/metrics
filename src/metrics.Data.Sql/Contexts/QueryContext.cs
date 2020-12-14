using System;
using System.Linq;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql.Contexts
{
    public class QueryContext : IQueryContext
    {
        private DbContext? _context;

        public QueryContext(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public IQueryable<T>? Query<T>() where T : class
        {
            return _context?.Set<T>();
        }

        public IQueryable<T>? RawSql<T>(string sql, params object[] parameters) where T : class
        {
            return _context?.Set<T>().FromSqlRaw(sql, parameters);
        }

        public IQueryable<T>? RawSql<T>(FormattableString sql) where T : class
        {
            return _context?.Set<T>().FromSqlInterpolated(sql);
        }


        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }

            _context = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private async Task DisposeAsyncCore()
        {
            if (_context != null) 
                await _context.DisposeAsync().ConfigureAwait(false);
            
            _context = null;
        }
    }
}