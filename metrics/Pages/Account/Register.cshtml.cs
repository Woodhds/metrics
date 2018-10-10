using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace metrics.Pages.Account
{
    public class Register : PageModel
    {
        private readonly UserManager<User> _userManager;

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
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Пароли должны совпадать")]
        public string ConfirmPassword { get; set; }

        [TempData]
        public string Message { get; set; }

        public Register(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var existed = await _userManager.FindByEmailAsync(Email);
                if (existed != null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким Email уже существует");
                    return Page();
                }
                var user = new User()
                {
                    Email = Email,
                    UserName = Email
                };
                var result = await _userManager.CreateAsync(user, Password);

                if(result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.USER_ROLE_NAME);
                    if(result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    }
                }
            }

            return Page();
        }
    }
}