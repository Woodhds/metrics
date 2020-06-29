using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface IRepository<T> where T: class, new()
    {
        Task<T> CreateAsync(T obj, CancellationToken ct = default);
        IQueryable<T> Read();
        Task<T> UpdateAsync(T obj, CancellationToken ct = default);
        Task<T> DeleteAsync(T odj, CancellationToken ct = default);
    }
}