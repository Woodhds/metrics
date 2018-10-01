using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> CreateAsync(TEntity entity);
        IQueryable<TEntity> Read();
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
