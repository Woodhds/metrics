using System;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface IResilientTransactionContext : IDisposable
    {
        Task ExecuteAsync(Func<ITransactionRepositoryContext, Task> context);
    }
}