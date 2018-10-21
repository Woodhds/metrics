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
        public IActionResult Repost(List<VkRepostViewModel> reposts, int timeout = 0)
        {
            try
            {
                _vkClient.Repost(reposts, timeout);
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