using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Base.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetPhoto(this IIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            var ci = identity as ClaimsIdentity;
            return ci?.FindFirst(d => d.Type == "photo")?.Value;
        }

        public static string GetId(this IIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            
            var ci = identity as ClaimsIdentity;
            return ci?.FindFirst(d => d.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}