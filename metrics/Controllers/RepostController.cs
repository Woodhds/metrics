﻿using metrics.Options;
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

namespace metrics.Controllers
{

    public class RepostController : Controller
    {
        private readonly VkontakteOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IVkClient _vkClient;

        public RepostController(IHttpClientFactory httpClientFactory, IOptions<VkontakteOptions> options, IVkClient vkClient)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _vkClient = vkClient;
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
            return View(new RepostsViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "VkPolicy")]
        public new async Task<IActionResult> User(RepostsViewModel model)
        {
            model.Messages = (await _vkClient.GetReposts(model.Filter)).Response.Items;
            return View(model);
        }
    }
}