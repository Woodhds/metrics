using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using metrics.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using metrics.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace metrics.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        private JwtBearerOptions _jwtBearerOptions;

        public HomeController(IHttpClientFactory httpClientFactory, IOptions<JwtBearerOptions> jwtBearerOptions)
        {
            _jwtBearerOptions = jwtBearerOptions.Value;
            _httpClientFactory = httpClientFactory;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Required]string token)
        {
            if (!ModelState.IsValid)
            {
                return Ok();
            }
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(VkontakteOptions.UserInformationEndpoint, new Dictionary<string, string>()
                {
                    ["v"] = Constants.ApiVersion,
                    ["access_token"] = token,
                    ["fields"] = string.Join(",", new List<string>() { "first_name", "last_name" })
                }));

                if (response.IsSuccessStatusCode)
                {
                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (payload.TryGetValue("error", out var error))
                    {
                        ModelState.AddModelError("", error.Value<string>());
                    }
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, payload["response"].First["id"]?.ToString()),
                        new Claim(Constants.VK_TOKEN_CLAIM, token),
                        new Claim(ClaimTypes.Name, $"{payload["response"].First["first_name"]} {payload["response"].First["last_name"]}")
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
                signingCredentials: new SigningCredentials(_jwtBearerOptions.TokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
