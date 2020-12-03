using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Abstractions
{
    public interface IEntityConfiguration
    {
        void Configure(ModelBuilder builder);
    }
}