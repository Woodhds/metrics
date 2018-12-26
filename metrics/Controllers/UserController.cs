using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.EF;
using Data.Entities;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    public class UserController : Controller
    {
        private readonly IRepository<VkUser> _vkUserRepository;
        public UserController(IRepository<VkUser> vkUserRepository)
        {
            _vkUserRepository = vkUserRepository;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult<IEnumerable<VkUser>>> Users()
        {
            var users = await _vkUserRepository.Read()
                .Select(c => new {c.FullName, c.Avatar, c.UserId}).ToListAsync();
            return Ok(users);
        }
    }
}