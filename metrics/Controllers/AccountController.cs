using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using metrics.Models;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private JwtOptions _jwtBearerOptions;

        public AccountController(SignInManager<User> signInManager, IHttpClientFactory httpClientFactory,
            IOptions<JwtOptions> jwtBearerOptions)
        {
            _httpClientFactory = httpClientFactory;
            _signInManager = signInManager;
            _jwtBearerOptions = jwtBearerOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<string>> Login([FromBody]LoginModel model)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(
                    VkontakteOptions.UserInformationEndpoint, new Dictionary<string, string>
                    {
                        ["v"] = Constants.ApiVersion,
                        ["access_token"] = model.Token,
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
                        new Claim(Constants.VK_TOKEN_CLAIM, model.Token),
                        new Claim(ClaimTypes.Name,
                            $"{payload["response"].First["first_name"]} {payload["response"].First["last_name"]}")
                    };

                    return Ok(new { accessToken = CreateToken(claims) });
                }
            }

            return Ok();
        }

        private string CreateToken(List<Claim> claims)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtBearerOptions.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtBearerOptions.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.ToString()));
            var token = new JwtSecurityToken(
                issuer: _jwtBearerOptions.Issuer,
                audience: _jwtBearerOptions.Audience,
                claims: claims,
                expires: DateTime.Now.Add(TimeSpan.FromHours(24)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerOptions.Key)),
                    SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}