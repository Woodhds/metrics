using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Extensions;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IVkUserService _vkUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IVkUserService vkUserService, IHttpContextAccessor httpContextAccessor)
        {
            _vkUserService = vkUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VkUserModel>>> Users(string searchStr = "")
        {
            return Ok(await _vkUserService.SearchAsync(searchStr));
        }

        [HttpPost]
        public async Task<ActionResult<VkUserModel>> Add([Required]string userId, CancellationToken ct = default)
        {
            return Ok(await _vkUserService.CreateAsync(userId, _httpContextAccessor.HttpContext.User.Identity.GetUserId(), ct));
        }
    }
}