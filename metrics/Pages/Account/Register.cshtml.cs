using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DAL.Entities;
using metrics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Web;

namespace metrics.Pages.Account
{
    public class Register : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        [BindProperty]
        public RegisterViewModel ViewModel { get; set; }

        [TempData]
        public string Message { get; set; }

        public Register(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var existed = await _userManager.FindByEmailAsync(ViewModel.Email);
                if (existed != null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким Email уже существует");
                    return Page();
                }
                var user = new User()
                {
                    Email = ViewModel.Email,
                    UserName = ViewModel.Email
                };
                var result = await _userManager.CreateAsync(user, ViewModel.Password);

                if(result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.USER_ROLE_NAME);
                    if(result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page("/Account/ConfirmEmail", null, 
                            new { token = token, userId = user.Id }, Request.Scheme);

                        await _emailService.SendAsync("Подтверждение электронного адреса",
                            $"Пожалуйста перейдите <a href='{HttpUtility.UrlEncode(callbackUrl)}'>по ссылке</a> для подтверждения электронного адреса", 
                            new List<string> { user.Email });
                        Message = "На ваш электронный ящик было отправлено письмо с подтверждением";
                    }
                }
            }

            return Page();
        }

        public class RegisterViewModel
        {
            [EmailAddress]
            [Required]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Display(Name = "Пароль")]
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Display(Name = "Повторите пароль")]
            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Пароли должны совпадать")]
            public string ConfirmPassword { get; set; }
        }
    }
}