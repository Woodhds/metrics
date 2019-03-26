using Data.EF;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DAL.Tests
{
    [TestFixture]
    public class AssemblyInitializer
    {
        public static DbContextOptions Options;

        [OneTimeSetUp]
        public static async Task Init()
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
