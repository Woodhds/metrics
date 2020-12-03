using System;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface IRawSqlTransactionContext : ITransactionContext
    {
        Task<int> ExecuteRawCommandAsync(FormattableString sql);
        Task<int> ExecuteRawCommandAsync(string sql, params object[] parameters);
    }
}