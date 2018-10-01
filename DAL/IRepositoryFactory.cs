using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public interface IRepositoryFactory<TContext> where TContext: DbContext
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
    }
}