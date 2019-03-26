using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Data.Entities;
using Data.Models;
using metrics.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace metrics.services.tests
{
    [TestFixture]
    public class UserControllerTests
    {
        [Test]
        public async Task UsersTests()
        {
            using(var context = new TestDataContext()) 
            {
                await context.Database.EnsureCreatedAsync();
                var repository = new Repository<VkUser>(context);
                var client = TestOptions.GetClient();
                var controller = new UserController(repository, client);
                var result = await controller.Users();
                var objectResult = result.Result as ObjectResult;
                Assert.IsInstanceOf<IEnumerable<VkUserModel>>(objectResult.Value);
                Assert.IsNotNull(objectResult.Value);
                var list = objectResult.Value as IEnumerable<VkUserModel>;
                Assert.IsTrue(list.Count() > 0);
            }
        }
    }
}