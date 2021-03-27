using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<T> UpdateProperties(T obj, Func<T, object> properties, CancellationToken ct = default)
        {
            _dbContext.ChangeTracker.TrackGraph(obj, state =>
            {
                state.Entry.State = EntityState.Unchanged;

                foreach (var prop in properties(obj).GetType().GetProperties())
                {
                    var propEntry = state.Entry.Property(prop.Name);
                    if (propEntry == null) continue;

                    propEntry.IsModified = true;
                }
            });

            await _dbContext.SaveChangesAsync(ct);

            return obj;
        }
    }
}