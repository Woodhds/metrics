using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using metrics.Extensions;
using metrics.Models;
using metrics.Options;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private JwtOptions _jwtBearerOptions;
        private readonly IVkClient _vkClient;

        public AccountController(IHttpClientFactory httpClientFactory, IOptions<JwtOptions> jwtBearerOptions, IVkClient vkClient)
        {
            _httpClientFactory = httpClientFactory;
            _jwtBearerOptions = jwtBearerOptions.Value;
            _vkClient = vkClient;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return LocalRedirect("/");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromForm] LoginModel model, string redirectUrl)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(
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

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, id),
                        new Claim(Constants.VK_TOKEN_CLAIM, model.Token),
                        new Claim(ClaimTypes.Name, name),
                        new Claim("photo", payload["response"].First["photo_50"].ToString())
                    };
                    var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(new ClaimsPrincipal(ci), new AuthenticationProperties());

                    return Redirect(Url.GetLocalUrl(redirectUrl));
                }
            }

            return View(model);
        }

        [Authorize(Policy = "VkPolicy")]
        public ActionResult<string> Token()
        {
            return Ok(CreateToken(User.Claims.ToList()));
        }

        public IActionResult Drop(string exclude = null, int count = 1000, int offset = 0)
        {
            var groups = _vkClient.GetGroups(count, offset)?.Response?.Items;
            if (!string.IsNullOrEmpty(exclude))
            {
                groups = groups.Where(c => !c.Name.ToLower().Contains(exclude.ToLower())).ToList();
            }
            foreach (var group in groups)
            {
                _vkClient.LeaveGroup(group.Id);
            }
            return Ok();
        }

        private string CreateToken(List<Claim> claims)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtBearerOptions.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtBearerOptions.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.ToString()));
            var token = new JwtSecurityToken(
                _jwtBearerOptions.Issuer,
                _jwtBearerOptions.Audience,
                claims,
                expires: DateTime.Now.Add(TimeSpan.FromHours(24)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerOptions.Key)),
                    SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}