using System;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql.Contexts
{
    public class RawSqlTransactionContext : TransactionContext, IRawSqlTransactionContext
    {
        private readonly DbContext _context;
        private bool _disposed;
        
        public RawSqlTransactionContext(DbContext context, IDbContextTransaction dbContextTransaction) : base(
            dbContextTransaction)
        {
            _context = context;
        }

        public Task<int> ExecuteRawCommandAsync(FormattableString sql)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(sql);
        }

        public Task<int> ExecuteRawCommandAsync(string sql, params object[] parameters)
        {
            return _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        private void Dispose(bool disposing)
        {
            if(_disposed)
                return;

            if (disposing)
            {
                _context?.Dispose();
            }

            _disposed = true;
        }

        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}