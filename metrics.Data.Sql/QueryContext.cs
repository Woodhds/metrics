using System;
using System.Linq;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql
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