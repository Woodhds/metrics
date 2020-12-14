using System;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql.Contexts
{
    public class RawSqlTransactionContext : TransactionContext, IRawSqlTransactionContext
    {
        private DbContext? _context;

        public RawSqlTransactionContext(DbContext context, IDbContextTransaction dbContextTransaction) : base(
            dbContextTransaction)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<int>? ExecuteRawCommandAsync(FormattableString sql)
        {
            return _context?.Database.ExecuteSqlInterpolatedAsync(sql);
        }

        public Task<int>? ExecuteRawCommandAsync(string sql, params object[] parameters)
        {
            return _context?.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }

            _context = null;
        }

        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public override async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync().ConfigureAwait(false);
            }

            _context = null;
            Dispose(false);
            await base.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}