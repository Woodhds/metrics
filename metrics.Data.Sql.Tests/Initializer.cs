using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace metrics.Data.Sql.Tests
{
    [SetUpFixture]
    public class Initializer
    {
        public static TransactionScopeFactory TransactionScopeFactory { get; set; }
        public static DataContextFactory DataContextFactory { get; set; }

        public Initializer()
        {
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            serviceCollection.AddSingleton<IEntityConfigurationProvider, EntityConfigurationProvider>();
            
            var builder = new DbContextOptionsBuilder()
                .UseNpgsql("Host=localhost;Port=5432;Database=test_ctx;UserId=postgres;Password=password")
                .UseApplicationServiceProvider(serviceCollection.BuildServiceProvider())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging();

            DataContextFactory = new DataContextFactory(builder.Options);
            TransactionScopeFactory = new TransactionScopeFactory(DataContextFactory);
        }
        
        [OneTimeSetUp]
        public void Setup()
        {
            DataContextFactory.Create().Database.EnsureCreated();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DataContextFactory.Create().Database.EnsureDeleted();
        }
    }
}