using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using DAL;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<VkUser> _vkUserRepository;
        private readonly IVkClient _vkClient;

        public UserController(IRepository<VkUser> vkUserRepository, IVkClient vkClient)
        {
            _vkUserRepository = vkUserRepository;
            _vkClient = vkClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VkUserModel>>> Users()
        {
            var users = await _vkUserRepository.Read()
                .Select(c => new VkUserModel 
                { 
                    FullName = c.FirstName + ' ' + c.LastName, 
                    Avatar = c.Avatar, 
                    UserId = c.UserId
                }
            ).ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Add([Required]string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userInfo = _vkClient.GetUserInfo(userId);
            var user = new VkUser { UserId = userId};
            if (_vkUserRepository.Read().Any(c => c.UserId == userId))
            {
                return BadRequest("Пользователь уже существует");
            }
            user.FirstName = userInfo.Response.First()?.First_name;
            user.LastName = userInfo.Response.First()?.Last_Name;
            user.Avatar = userInfo.Response.First()?.Photo_50;
            await _vkUserRepository.CreateAsync(user);
            return Ok();
        }
    }
}