using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DAL
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<CommittableTransaction> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task<TEntity> CreateAsync(TEntity entity);
        IQueryable<TEntity> Read();
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task<TEntity> Find(int id);
    }
}
