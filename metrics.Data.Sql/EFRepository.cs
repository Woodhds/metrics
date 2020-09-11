using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql
{
    public class EFRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly DbSet<T> _set;
        private readonly DbContext _dbContext;
        
        public EFRepository(DbContext dbContext)
        {
            _set = dbContext.Set<T>();
            _dbContext = dbContext;
        }
        
        public async Task<T> CreateAsync(T obj, CancellationToken ct = default)
        {
            _set.Attach(obj).State = EntityState.Added;

            await _dbContext.SaveChangesAsync(ct);

            return obj;
        }

        public async Task<T> UpdateAsync(T obj, CancellationToken ct = default)
        {
            _set.Attach(obj).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync(ct);

            return obj;
        }

        public async Task<T> DeleteAsync(T obj, CancellationToken ct = default)
        {
            _set.Attach(obj).State = EntityState.Deleted;

            await _dbContext.SaveChangesAsync(ct);

            return obj;
        }
    }
}