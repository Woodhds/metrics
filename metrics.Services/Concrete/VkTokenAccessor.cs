using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using metrics.Services.Abstractions;
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
        
        public Task<string> GetTokenAsync(int? userId = null)
        {
            var ci = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            
            return Task.FromResult(ci == null ? string.Empty : ci.Claims.FirstOrDefault(c => c.Type == Constants.VkTokenClaim)?.Value);
        }
    }
}