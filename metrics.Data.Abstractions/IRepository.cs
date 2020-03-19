using System.Linq;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface IRepository<T> where T: class, new()
    {
        Task<T> CreateAsync(T obj);
        IQueryable<T> Read();
        Task<T> UpdateAsync(T obj);
        Task<T> DeleteAsync(T odj);
    }
}