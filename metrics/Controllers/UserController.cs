using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<ActionResult<IEnumerable<VkUserModel>>> Users(string searchStr = "")
        {
            return Ok(await _vkUserService.SearchAsync(searchStr));
        }

        [HttpPost]
        public async Task<ActionResult<VkUserModel>> Add([Required]string userId)
        {
            return Ok(await _vkUserService.CreateAsync(userId));
        }
    }
}