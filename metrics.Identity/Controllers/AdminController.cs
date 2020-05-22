using System.Linq;
using metrics.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Identity.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IdentityContext _identityContext;
        public AdminController(IdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        [HttpGet("usertoken/{userId}")]
        public ActionResult<string> GetUserToken(int userId)
        {
            return _identityContext.UserTokens.FirstOrDefault(f => f.UserId == userId && f.Name == "access_token")?.Value;
        }
    }
}