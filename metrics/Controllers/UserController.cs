using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Data.EF;
using Data.Entities;
using DAL;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    public class UserController : Controller
    {
        private readonly IRepository<VkUser> _vkUserRepository;
        private readonly IVkClient _vkClient;
        private readonly DataContext _dataContext;
        public UserController(IRepository<VkUser> vkUserRepository, IVkClient vkClient, DataContext dataContext)
        {
            _vkUserRepository = vkUserRepository;
            _vkClient = vkClient;
            _dataContext = dataContext;
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

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([Required]string userId)
        {
            if (!ModelState.IsValid)
                return View();

            var userInfo = _vkClient.GetUserInfo(userId);
            var user = new VkUser { UserId = userId};
            if (_dataContext.Set<VkUser>().Any(c => c.UserId == userId))
            {
                ModelState.AddModelError("", "Пользователь уже существует");
                return View();
            }
            user.FirstName = userInfo.Response.First()?.First_name;
            user.LastName = userInfo.Response.First()?.Last_Name;
            user.Avatar = userInfo.Response.First()?.Photo_50;
            _dataContext.Entry(user).State = EntityState.Added;
            await _dataContext.SaveChangesAsync();
            ViewBag.Result = "Пользователь добавлен";
            return View();
        }
    }
}