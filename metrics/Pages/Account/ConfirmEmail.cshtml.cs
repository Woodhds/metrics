using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using metrics.Options;
using Microsoft.Extensions.Options;
using metrics.Services.Abstract;

namespace metrics.Pages.Account
{
    public class ConfirmEmail : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGoogleRecaptchaService _recaptchaService;
        private readonly IUserManagerService _userManagerService;
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }
        [TempData]
        public string Message { get; set; }
        public ConfirmEmail(UserManager<User> userManager, IHttpClientFactory httpClientFactory,
            IGoogleRecaptchaService googleRecaptchaService, IUserManagerService userManagerService)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
            _recaptchaService = googleRecaptchaService;
            _userManagerService = userManagerService;
        }

        public async Task<IActionResult> OnGetAsync(string token, string userId)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                    return Page();
                }
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        return LocalRedirect("/");
                    }
                    ModelState.AddModelError("", "Ощибка подтверждения почты");
                }
            }
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!Request.Form.ContainsKey("g-recaptcha-response"))
            {
                ModelState.AddModelError("", "Капча не прошла валидацию");
                return Page();
            }

            if (ModelState.IsValid)
            {
                var response = await _recaptchaService.ValidateAsync(Request.Form["g-recaptcha-response"]);
                if(!response)
                {
                    ModelState.AddModelError("", "Капча не прошла валидацию");
                    return Page();
                }
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    return Page();
                }
                await _userManagerService.SendEmailConfirmation(user);
                Message = "Проверьте почту";
            }

            return Page();
        }
    }
}