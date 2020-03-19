using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.JsonWebTokens;

namespace metrics.Extensions
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this IIdentity identity)
        {
            if (!(identity is ClaimsIdentity ci))
            {
                throw new AuthenticationException();
            }

            var claim = ci.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                return int.Parse(claim.Value);
            }
            
            throw new AuthenticationException();
        }
    }
}