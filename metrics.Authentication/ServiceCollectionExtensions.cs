using System;
using System.Text;
using System.Threading.Tasks;
using metrics.Authentication.Options;
using metrics.Authentication.Services.Abstract;
using metrics.Authentication.Services.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace metrics.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMetricsAuthentication(this IServiceCollection services,
            IConfiguration configuration, Action<AuthenticationBuilder> action = null)
        {
            var jwtOptions = new JwtOptions();
            configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
            services.Configure<JwtOptions>(z =>
            {
                z.Audience = jwtOptions.Audience;
                z.Issuer = jwtOptions.Issuer;
                z.Key = jwtOptions.Key;
            });
            var authentication = services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            authentication.AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience
                    };
                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            
            action?.Invoke(authentication);

            services.AddSingleton<IJsonWebTokenGenerationService, JsonWebTokenGenerationService>();

            return services;
        }
    }
}