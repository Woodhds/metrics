using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Identity.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
    }
}