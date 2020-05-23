using System.Threading.Tasks;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Identity.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly UserStore _userStore;
        public AdminController(UserManager<User> userManager, UserStore userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
        }

        [HttpGet("usertoken/{userId}")]
        public async Task<ActionResult<string>> GetUserToken(int userId)
        {
            var user = await _userManager.FindByNameAsync(userId.ToString());
            if (user == null)
                return string.Empty;

            return await _userStore.GetTokenAsync(user, "Vkontakte", "access_token_implicit", default);
        }
    }
}