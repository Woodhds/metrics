using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Entities;
using metrics.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace metrics.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private JwtBearerOptions _jwtBearerOptions;

        public AccountController(SignInManager<User> signInManager, IHttpClientFactory httpClientFactory,
            IOptions<JwtBearerOptions> jwtBearerOptions)
        {
            _httpClientFactory = httpClientFactory;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }

        public async Task<IActionResult> Login(string token)
        {
            if (!ModelState.IsValid)
            {
                return Ok();
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(
                    VkontakteOptions.UserInformationEndpoint, new Dictionary<string, string>
                    {
                        ["v"] = Constants.ApiVersion,
                        ["access_token"] = token,
                        ["fields"] = string.Join(",", new List<string> {"first_name", "last_name"})
                    }));

                if (response.IsSuccessStatusCode)
                {
                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (payload.TryGetValue("error", out var error))
                    {
                        ModelState.AddModelError("", error.Value<string>());
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, payload["response"].First["id"]?.ToString()),
                        new Claim(Constants.VK_TOKEN_CLAIM, token),
                        new Claim(ClaimTypes.Name,
                            $"{payload["response"].First["first_name"]} {payload["response"].First["last_name"]}")
                    };

                    return Ok(CreateToken(claims));
                }
            }

            return Ok();
        }

        private string CreateToken(List<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _jwtBearerOptions.ClaimsIssuer,
                audience: _jwtBearerOptions.Audience,
                claims: claims,
                expires: DateTime.MaxValue,
                signingCredentials: new SigningCredentials(_jwtBearerOptions.TokenValidationParameters.IssuerSigningKey,
                    SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}