using System.Linq;
using System.Security.Claims;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Http;

namespace metrics.Services.Concrete
{
    public class VkTokenAccessor : IVkTokenAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public VkTokenAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public string GetToken()
        {
            var ci = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (ci == null)
            {
                return string.Empty;
            }
            return ci.Claims.FirstOrDefault(c => c.Type == Constants.VK_TOKEN_CLAIM)?.Value;
        }
    }
}