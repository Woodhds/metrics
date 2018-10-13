using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Entities;
using metrics.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace metrics.Pages.Account
{
    public class Login : PageModel
    {
        private readonly UserManager<User> _userManager;

        [Display(Name = "Email")]
        [EmailAddress]
        [Required]
        [BindProperty]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [Required]
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public Login(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            await HttpContext.SignOutAsync();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    return Page();
                }

                if (!user.EmailConfirmed)
                {
                    return RedirectToPage("ConfirmEmail");
                }

                if (!(await _userManager.CheckPasswordAsync(user, Password)))
                {
                    ModelState.AddModelError(string.Empty, "Указан неверный пароль");
                    return Page();
                }
                var ci = await CreateIdentity(user);
                await HttpContext.SignInAsync(ci);
                return LocalRedirect(Url.GetLocalUrl(ReturnUrl));
            }

            return Page();
        }

        private async Task<ClaimsPrincipal> CreateIdentity(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var ci = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return ci;
        }
    }
}