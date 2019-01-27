using Data.EF;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DAL.Tests
{
    [TestClass]
    public class AssemblyInitializer
    {
        public static DbContextOptions Options;

        [AssemblyInitialize]
        public static async Task Init(TestContext context)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite("Data Source=testdb.db;");
            Options = builder.Options;
            using (var dataContext = new DataContext(Options))
            {
                await dataContext.Database.EnsureDeletedAsync();
                await dataContext.Database.EnsureCreatedAsync();
                await dataContext.AddAsync(new VkUser());
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
