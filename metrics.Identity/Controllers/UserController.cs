using System.Security.Claims;
using System.Threading.Tasks;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Identity.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserStore _userStore;
        private readonly UserManager<User> _userManager;

        public UserController(IHttpContextAccessor httpContextAccessor, UserStore userStore, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userStore = userStore;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Id = _httpContextAccessor.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.NameIdentifier).Value,
                FullName = _httpContextAccessor.HttpContext.User.Identity.Name,
                Avatar = _httpContextAccessor.HttpContext.User.FindFirst(f => f.Type == "photo").Value,
            });
        }

        [HttpPost("token")]
        public async Task<IActionResult> SetToken(string token)
        {
            var user = await GetUser();
            await _userStore.SetTokenAsync(user, "Vkontakte", "access_token_implicit", token, default);
            return Ok();
        }

        [NonAction]
        private Task<User> GetUser()
        {
            return _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User
                .FindFirst(f => f.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}