using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DAL.Entities;
using metrics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Web;
using DAL;
using System.Transactions;
using metrics.Services.Abstract;

namespace metrics.Pages.Account
{
    public class Register : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserManagerService _userManagerService;
        [BindProperty]
        public RegisterViewModel ViewModel { get; set; }

        [TempData]
        public string Message { get; set; }

        public Register(UserManager<User> userManager, IUserManagerService userManagerService)
        {
            _userManager = userManager;
            _userManagerService = userManagerService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRoleAsync(user, Constants.USER_ROLE_NAME);
                        await _userManagerService.SendEmailConfirmation(user);
                        if (result.Succeeded)
                        {
                            Message = "На ваш электронный ящик было отправлено письмо с подтверждением";
                            transactionScope.Complete();
                        }
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