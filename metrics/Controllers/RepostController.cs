using metrics.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using metrics.Services.Abstract;
using metrics.Models;
using System.Linq;
using metrics.Services.Models;
using Microsoft.Extensions.Logging;

namespace metrics.Controllers
{

    public class RepostController : Controller
    {
        private readonly VkontakteOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IVkClient _vkClient;
        private readonly ILogger<RepostController> _logger;
        public RepostController(IHttpClientFactory httpClientFactory, IOptions<VkontakteOptions> options, 
            IVkClient vkClient, ILogger<RepostController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _vkClient = vkClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Required]string token)
        {
            if (!ModelState.IsValid)
            {
                return View();
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
                    await HttpContext.SignOutAsync();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)), 
                        new AuthenticationProperties() { ExpiresUtc = DateTimeOffset.MaxValue });
                    return View();
                }
            }
            ModelState.AddModelError("", "Ошибка аутентификации");
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "VkPolicy")]
        public new IActionResult User()
        {
            return View();
        }

        [Authorize(Policy = "VkPolicy")]
        public IActionResult GetData(string userId, int skip, int take, string search = null)
        {
            var data = _vkClient.GetReposts(userId, skip, take, search);
            var reposts = data.Response
                .Items.OrderByDescending(c => DateTimeOffset.FromUnixTimeSeconds(c.date))
                .Where(c => c.Copy_History != null && c.Copy_History.Count > 0).Select(c => c.Copy_History.First()).Distinct().ToList();
            return Ok(
                new {
                    Data = reposts,
                    Total = data.Response.Count
                }
            );
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpPost]
        public IActionResult Repost(List<VkRepostViewModel> reposts)
        {
            try
            {
                _vkClient.Repost(reposts);
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest();
            }
        }
    }
}