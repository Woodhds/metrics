using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Abstractions
{
    public interface IDataContextFactory
    {
        DbContext Create();
    }
}