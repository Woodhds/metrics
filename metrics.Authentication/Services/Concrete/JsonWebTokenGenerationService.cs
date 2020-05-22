using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using metrics.Authentication.Options;
using metrics.Authentication.Services.Abstract;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace metrics.Authentication.Services.Concrete
{
    public class JsonWebTokenGenerationService : IJsonWebTokenGenerationService
    {
        private readonly JwtOptions _jwtOptions;

        public JsonWebTokenGenerationService(IOptionsMonitor<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.CurrentValue;
        }

        public string Generate(ClaimsPrincipal principal)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                principal.Claims,
                null,
                DateTime.Now.AddDays(14),
                signInCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}