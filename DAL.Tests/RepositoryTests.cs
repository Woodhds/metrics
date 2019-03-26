using Data.EF;
using Data.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DAL.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private Repository<VkUser> _repository;

        public RepositoryTests()
        {
            var dataContext = new DataContext(AssemblyInitializer.Options);
            _repository = new Repository<VkUser>(dataContext);
        }

        [Test]
        public void ReadTest()
        {
            var count = _repository.Read().Count();
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void CreateTest()
        {
            var entity = _repository.CreateAsync(new VkUser()).GetAwaiter().GetResult();
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id != 0);
        }

        [Test]
        public void DeleteTestFalse()
        {
            _repository.DeleteAsync(34).GetAwaiter().GetResult();
            var count = _repository.Read().Count();
            Assert.IsFalse(count == 0);
        }

        [Test]
        public void DeleteTestTrue()
        {
            _repository.DeleteAsync(1).GetAwaiter().GetResult();
            var count = _repository.Read().Count();
            Assert.AreEqual(count, 0);
        }

        [Test]
        public void UpdateTestTrue()
        {
            var obj = _repository.Find(1).GetAwaiter().GetResult();
            obj.FirstName = "Test";
            obj = _repository.UpdateAsync(obj).GetAwaiter().GetResult();
            Assert.AreEqual(obj.FirstName, "Test");
        }
        
        [Test]
        public void UpdateTestFalse()
        {
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
                await _repository.UpdateAsync(new VkUser {Id = 32, FirstName = "Test"}));
        }
    }
}
