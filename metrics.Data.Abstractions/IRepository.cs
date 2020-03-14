using System.Linq;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface IRepository<T> where T: class
    {
        Task<T> CreateAsync(T obj);
        IQueryable<T> Read();
        Task<T> UpdateAsync(T obj);
        Task<T> DeleteAsync(int id);
    }
}