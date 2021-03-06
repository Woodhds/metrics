﻿using System.Threading.Tasks;
using metrics.Authentication;
using metrics.Authentication.Services.Abstract;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace metrics.Infrastructure
{
    public class IdentityUserMiddleware
    {
        private readonly RequestDelegate _next;

        public IdentityUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISecurityUserManager userManager)
        {
            if (!context.User.Identity.IsAuthenticated)
                await _next(context);

            using (userManager.SetUser(new SecurityUser
            {
                Id = context.User.Identity.GetUserId()
            }))
            {
                await _next(context);
            }
        }
    }
}