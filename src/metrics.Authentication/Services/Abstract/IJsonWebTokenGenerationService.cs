using System.Security.Claims;

namespace metrics.Authentication.Services.Abstract
{
    public interface IJsonWebTokenGenerationService
    {
        string Generate(ClaimsPrincipal principal);
    }
}