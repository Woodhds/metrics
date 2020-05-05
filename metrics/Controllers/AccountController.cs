using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Models;
using metrics.Options;
using metrics.Services;
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
        private readonly IMessageBroker _messageBroker;

        public AccountController(
            IHttpClientFactory httpClientFactory,
            IVkClient vkClient,
            IOptions<JwtOptions> jwtOptions,
            IMessageBroker messageBroker
        )
        {
            _httpClient = httpClientFactory.CreateClient();
            _vkClient = vkClient;
            _messageBroker = messageBroker;
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
        public async Task<ActionResult> Login([FromBody] LoginModel model,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(
                VkontakteOptions.UserInformationEndpoint, new Dictionary<string, string>
                {
                    ["v"] = Constants.ApiVersion,
                    ["access_token"] = model.Token,
                    ["fields"] = string.Join(",", new List<string> {"first_name", "last_name", "photo_50"})
                }), cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Ok();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload.TryGetValue("error", out var error))
            {
                return BadRequest(error);
            }

            var id = payload["response"].First["id"]?.ToString();

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id not valid");
            }

            var name = $"{payload["response"].First["first_name"]} {payload["response"].First["last_name"]}";
            var avatar = payload["response"].First["photo_50"].ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, id),
                new Claim(Constants.VkTokenClaim, model.Token),
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

            await _messageBroker.PublishAsync(new LoginEvent
                {
                    Token = model.Token, 
                    UserId = int.Parse(id)
                },
                cancellationToken);
            
            return Ok(new
            {
                Id = id,
                Avatar = avatar,
                FullName = name,
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken)
            });
        }

        [HttpPost("drop")]
        public async Task<IActionResult> Drop(
            string exclude = null,
            string identity = null,
            int count = 1000,
            int offset = 0
        )
        {
            var total = 0;
            var workCount = count;
            var workOffset = offset;
            var groups = new List<VkGroup>();
            do
            {
                var response = await _vkClient.GetGroups(workCount, workOffset);
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
                await _vkClient.LeaveGroup(group.Id);
            }

            return Ok();
        }
    }
}