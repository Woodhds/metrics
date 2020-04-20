namespace metrics.Data.Sql
{
    public interface IDataContextFactory
    {
        DataContext Create();
    }
}