using System.Linq;
using System.Threading.Tasks;
using Base.Contracts.Options;
using metrics.Authentication.Services.Abstract;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace metrics.Identity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserStore _userStore;
        private readonly IJsonWebTokenGenerationService _jsonWebTokenGenerationService;
        private readonly IMessageBroker _messageBroker;
        private readonly VkontakteOptions _vkontakteOptions;

        public AuthorizeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            UserStore userStore,
            IJsonWebTokenGenerationService jsonWebTokenGenerationService,
            IMessageBroker messageBroker,
            IOptions<VkontakteOptions> options
        )
        {
            _vkontakteOptions = options.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _jsonWebTokenGenerationService = jsonWebTokenGenerationService;
            _messageBroker = messageBroker;
        }

        [HttpGet("externallogin")]
        public IActionResult ExternalLogin(string loginProvider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return BadRequest();

            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(loginProvider,
                Url.Action("ExternalLoginCallback"));

            authenticationProperties.Items["returnUrl"] = returnUrl;

            return Challenge(authenticationProperties, loginProvider);
        }

        [HttpGet("externallogincallback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
                return BadRequest();

            var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

            var accessToken = string.Empty;

            if (user == null)
            {
                user = new User
                {
                    UserName = loginInfo.ProviderKey
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                    return BadRequest(createResult.Errors);

                await _userManager.AddLoginAsync(user, loginInfo);
            }

            var userToken = loginInfo.AuthenticationTokens.FirstOrDefault(d => d.Name == "access_token");
            if (userToken != null)
            {
                accessToken = userToken.Value;
                await _userStore.SetTokenAsync(user, loginInfo.LoginProvider, userToken.Name, userToken.Value,
                    default);
            }

            var returnUrl = loginInfo.AuthenticationProperties.Items["returnUrl"];

            returnUrl = QueryHelpers.AddQueryString(returnUrl, "access_token",
                _jsonWebTokenGenerationService.Generate(loginInfo.Principal));

            await _messageBroker.PublishAsync(new LoginEvent {Token = accessToken, UserId = user.Id});

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }


        [HttpGet("externalToken")]
        [AllowAnonymous]
        public ActionResult GetExternalToken()
        {
            var url = QueryHelpers.AddQueryString(VkApiUrls.AuthorizeUrl, "client_id", _vkontakteOptions.AppId);
            url = QueryHelpers.AddQueryString(url, "scope", _vkontakteOptions.AppScope);
            url = QueryHelpers.AddQueryString(url, "response_type", "token");
            url = QueryHelpers.AddQueryString(url, "redirect_uri", "https://oauth.vk.com/blank.html");
            return Redirect(url);
        }
    }
}