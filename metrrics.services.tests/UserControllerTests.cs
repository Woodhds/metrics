using metrics.Services.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using metrics.Controllers;
using DAL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Data.Entities;
using metrrics.services.tests;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Data.Models;
using System;

namespace metrics.test
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
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
                Assert.IsInstanceOfType(objectResult.Value, typeof(IEnumerable<VkUserModel>));
                Assert.IsNotNull(objectResult.Value);
                var list = objectResult.Value as IEnumerable<VkUserModel>;
                Assert.IsTrue(list.Count() > 0);
            }
        }
    }
}