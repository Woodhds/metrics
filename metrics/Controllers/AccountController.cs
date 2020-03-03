using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Models;
using metrics.Options;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "VkPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IVkClient _vkClient;
        private readonly JwtOptions _jwtOptions;

        public AccountController(IHttpClientFactory httpClientFactory, IVkClient vkClient, IOptions<JwtOptions> jwtOptions)
        {
            _httpClient = httpClientFactory.CreateClient();
            _vkClient = vkClient;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(
                VkontakteOptions.UserInformationEndpoint, new Dictionary<string, string>
                {
                    ["v"] = Constants.ApiVersion,
                    ["access_token"] = model.Token,
                    ["fields"] = string.Join(",", new List<string> {"first_name", "last_name", "photo_50"})
                }));

            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                if (payload.TryGetValue("error", out var error))
                {
                    ModelState.AddModelError("", error.Value<string>());
                }

                var id = payload["response"].First["id"]?.ToString();
                var name = $"{payload["response"].First["first_name"]} {payload["response"].First["last_name"]}";
                var avatar = payload["response"].First["photo_50"].ToString();

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, id),
                    new Claim(Constants.VK_TOKEN_CLAIM, model.Token),
                    new Claim(JwtRegisteredClaimNames.GivenName, name),
                    new Claim("photo", avatar)
                };
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
                var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var securityToken = new JwtSecurityToken(
                    _jwtOptions.Issuer,
                    _jwtOptions.Audience,
                    claims,
                    null,
                    DateTime.Now.AddDays(14),
                    signInCredentials
                );
                return Ok(new
                {
                    Id = id,
                    Avatar = avatar,
                    FullName = name,
                    Token = new JwtSecurityTokenHandler().WriteToken(securityToken)
                });
            }

            return Ok();
        }
        
        [HttpPost("drop")]
        public IActionResult Drop(string exclude = null, string identity = null, int count = 1000, int offset = 0)
        {
            var total = 0;
            var workCount = count;
            var workOffset = offset;
            var groups = new List<VkGroup>();
            do
            {
                var response = _vkClient.GetGroups(workCount, workOffset);
                groups.AddRange(response?.Response?.Items ?? new List<VkGroup>());
                total = response?.Response?.Count ?? 0;
                workOffset += workCount;
            } while (total <= workOffset + workCount);
            if (!string.IsNullOrEmpty(exclude))
            {
                groups = groups.Where(c => !c.Name.ToLower().Contains(exclude.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(identity))
            {
                var ids = identity.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                groups = groups.Where(d => ids.Contains(d.Id)).ToList();
            }
            foreach (var group in groups)
            {
                _vkClient.LeaveGroup(group.Id);
            }
            return Ok();
        }
    }
}