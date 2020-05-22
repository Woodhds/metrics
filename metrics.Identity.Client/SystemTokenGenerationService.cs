using System.Collections.Generic;
using System.Security.Claims;
using metrics.Authentication.Services.Abstract;
using metrics.Identity.Client.Abstractions;

namespace metrics.Identity.Client
{
    public class SystemTokenGenerationService : ISystemTokenGenerationService
    {
        private readonly IJsonWebTokenGenerationService _jsonWebTokenGenerationService;

        public SystemTokenGenerationService(IJsonWebTokenGenerationService jsonWebTokenGenerationService)
        {
            _jsonWebTokenGenerationService = jsonWebTokenGenerationService;
        }

        public string GetSystemToken()
        {
            var claims = new List<Claim> {new Claim(ClaimTypes.Role, "Admin")};
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return _jsonWebTokenGenerationService.Generate(principal);
        }
    }
}