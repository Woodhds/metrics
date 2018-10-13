using DAL.Entities;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using metrics.Services.Helpers;

namespace metrics.Services.Concrete
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly HttpContext _httpContext;
        public UserManagerService(UserManager<User> userManager, IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContext = httpContextAccessor.HttpContext;
            _emailService = emailService;

        }

        public async Task SendEmailConfirmation(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var query = new NameValueCollection()
            {
                { "token", HttpUtility.UrlEncode(token) },
                { "userId", user.Id.ToString() }
            }.BuildUrl(new UriBuilder(_httpContext.Request.Scheme,
                _httpContext.Request.Host.Host, 
                _httpContext.Request.Host.Port.Value, 
                "Account/ConfirmEmail"
            ).ToString());

            await _emailService.SendAsync("Подтверждение электронного адреса",
                $"Пожалуйста перейдите <a href='{query}'>по ссылке</a> для подтверждения электронного адреса",
                new List<string> { user.Email });
        }
    }
}
