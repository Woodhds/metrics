using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly IMessageBroker _messageBroker;

        public UserController(IHttpContextAccessor httpContextAccessor, UserStore userStore,
            UserManager<User> userManager, IMessageBroker messageBroker)
        {
            _httpContextAccessor = httpContextAccessor;
            _userStore = userStore;
            _userManager = userManager;
            _messageBroker = messageBroker;
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

        [HttpGet("token")]
        public async Task<IActionResult> GetTokens()
        {
            var user = await GetUser();
            return Ok(
                await _userStore.Context.UserTokens
                    .Where(f => f.UserId == user.Id)
                    .Select(f => new {f.Value, f.Name, f.LoginProvider})
                    .ToArrayAsync()
            );
        }

        [HttpDelete("token")]
        public async Task<IActionResult> DeleteToken(string name, string loginProvider)
        {
            var user = await GetUser();
            await _userStore.RemoveTokenAsync(user, loginProvider, name, CancellationToken.None);
            await _messageBroker.PublishAsync(new UserTokenRemoved
            {
                UserId = user.Id,
                Name = name
            });
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