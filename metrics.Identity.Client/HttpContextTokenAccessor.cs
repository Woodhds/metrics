using System.Threading.Tasks;
using metrics.Identity.Client.Abstractions;
using metrics.Services.Abstractions;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace metrics.Identity.Client
{
    public class HttpContextTokenAccessor : IVkTokenAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheTokenAccessor _cacheTokenAccessor;

        public HttpContextTokenAccessor(IHttpContextAccessor httpContextAccessor,
            ICacheTokenAccessor cacheTokenAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheTokenAccessor = cacheTokenAccessor;
        }

        public Task<string> GetTokenAsync(int? userId = null)
        {
            return _httpContextAccessor.HttpContext?.User?.Identity == null
                ? Task.FromResult("")
                : _cacheTokenAccessor.GetTokenAsync(_httpContextAccessor.HttpContext.User.Identity.GetUserId());
        }
    }
}