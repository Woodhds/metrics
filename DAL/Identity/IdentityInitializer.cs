using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DAL.Identity
{
    public static class IdentityInitializer
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            if (!roleManager.RoleExistsAsync(Constants.USER_ROLE_NAME).Result)
            {
                roleManager.CreateAsync(new Role() { Name = Constants.USER_ROLE_NAME }).GetAwaiter().GetResult();
            }
        }
    }
}
