using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}