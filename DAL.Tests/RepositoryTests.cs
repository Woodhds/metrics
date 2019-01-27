using Data.EF;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DAL.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private Repository<VkUser> _repository;

        public RepositoryTests()
        {
            var dataContext = new DataContext(AssemblyInitializer.Options);
            _repository = new Repository<VkUser>(dataContext);
        }

        [TestMethod]
        public void ReadTest()
        {
            var count = _repository.Read().Count();
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void CreateTest()
        {
            var entity = _repository.CreateAsync(new VkUser()).GetAwaiter().GetResult();
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id != 0);
        }

        [TestMethod]
        public void DeleteTestFalse()
        {
            _repository.DeleteAsync(34).GetAwaiter().GetResult();
            var count = _repository.Read().Count();
            Assert.IsFalse(count == 0);
        }

        [TestMethod]
        public void DeleteTestTrue()
        {
            _repository.DeleteAsync(1).GetAwaiter().GetResult();
            var count = _repository.Read().Count();
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void UpdateTestTrue()
        {
            var obj = _repository.Find(1).GetAwaiter().GetResult();
            obj.FirstName = "Test";
            obj = _repository.UpdateAsync(obj).GetAwaiter().GetResult();
            Assert.AreEqual(obj.FirstName, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void UpdateTestFalse()
        {
            var obj = _repository.UpdateAsync(new VkUser() { Id = 32, FirstName = "Test" }).GetAwaiter().GetResult();
        }
    }
}
