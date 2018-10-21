using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using metrics.Models;
using DAL;
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

namespace metrics.Controllers
{
    public class HomeController : Controller
    {
        private IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

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

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
