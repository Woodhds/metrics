using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL
{
    public class Repository<TEntity> : IRepository<TEntity> 
        where TEntity : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _set;

        public Repository(DbContext context)
        {
            _context = context;
            _set = _context.Set<TEntity>();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await Find(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TEntity> Find(int id)
        {
            return await _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<TEntity> Read()
        {
            return _set.AsNoTracking().AsQueryable();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entityEntry = _set.Update(entity);
            await _context.SaveChangesAsync();
            return await Task.FromResult(entityEntry.Entity);
        }
    }
}