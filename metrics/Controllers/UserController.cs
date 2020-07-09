using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IVkUserService _vkUserService;

        public UserController(IVkUserService vkUserService)
        {
            _vkUserService = vkUserService;
        }

        [HttpGet]
        public Task<IEnumerable<VkUserModel>> Users(string searchStr = "")
        {
            return _vkUserService.Get(searchStr);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string search)
        {
            return Ok((await _vkUserService.SearchAsync(search))?.Response?.Items?.Select(q =>
                new VkUserModel
                {
                    Avatar = q.Photo_50,
                    Id = q.Id,
                    FullName = q.First_name + " " + q.Last_Name
                }));
        }

        [HttpPost]
        public Task<VkUserModel> Add([Required] string userId)
        {
            return _vkUserService.CreateAsync(userId);
        }
    }
}