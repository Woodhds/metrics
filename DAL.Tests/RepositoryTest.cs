using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Tests
{
    public class RepositoryTest<TEntity> : IRepository<TEntity> where TEntity: BaseEntity
    {
        private List<TEntity> entities = new List<TEntity>();
        
        public Task<IDbContextTransaction> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            throw new System.NotImplementedException();
        }

        public Task<TEntity> CreateAsync(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TEntity> Read()
        {
            throw new System.NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<TEntity> Find(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}