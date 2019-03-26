using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using metrics.Extensions;
using metrics.Models;
using metrics.Options;
using metrics.Services.Abstract;
using metrics.Services.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IVkClient _vkClient;

        public AccountController(IHttpClientFactory httpClientFactory, IVkClient vkClient)
        {
            _httpClientFactory = httpClientFactory;
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

        [HttpGet]
        public IActionResult Drop()
        {
            return View();
        }
        
        [HttpPost]
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