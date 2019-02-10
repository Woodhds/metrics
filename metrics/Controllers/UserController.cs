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
using Data.Models;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    public class UserController : Controller
    {
        private readonly IRepository<VkUser> _vkUserRepository;
        private readonly IVkClient _vkClient;

        public UserController(IRepository<VkUser> vkUserRepository, IVkClient vkClient)
        {
            _vkUserRepository = vkUserRepository;
            _vkClient = vkClient;
        }
        
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VkUserModel>>> Users()
        {
            var users = await _vkUserRepository.Read()
                .Select(c => new VkUserModel 
                { 
                    FullName = c.FullName, 
                    Avatar = c.Avatar, 
                    UserId = c.UserId
                }
            ).ToListAsync();

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
            if (_vkUserRepository.Read().Any(c => c.UserId == userId))
            {
                ModelState.AddModelError("", "Пользователь уже существует");
                return View();
            }
            user.FirstName = userInfo.Response.First()?.First_name;
            user.LastName = userInfo.Response.First()?.Last_Name;
            user.Avatar = userInfo.Response.First()?.Photo_50;
            await _vkUserRepository.CreateAsync(user);
            ViewBag.Result = "Пользователь добавлен";
            return View();
        }
    }
}