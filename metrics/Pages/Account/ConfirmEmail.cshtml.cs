using metrics.Extensions;
using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace metrics.Pages.Account
{
    public class ConfirmEmail : PageModel
    {
        private readonly UserManager<User> _userManager;
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public ConfirmEmail(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string token, string userId)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ErrorMessage = "Пользователь не найден";
                    return Page();
                }
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        return LocalRedirect("/");
                    }
                }
                return LocalRedirect("/");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(Email);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    return Page();
                }
            }

            return Page();
        }
    }
}