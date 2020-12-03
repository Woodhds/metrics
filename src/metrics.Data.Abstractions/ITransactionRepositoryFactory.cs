namespace metrics.Data.Abstractions
{
    public interface ITransactionRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class, new();
    }
}