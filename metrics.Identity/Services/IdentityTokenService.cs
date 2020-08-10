using System.Threading.Tasks;
using Grpc.Core;
using metrics.Identity.Client.Abstractions;
using metrics.Identity.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace metrics.Identity.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class IdentityTokenService : Client.Abstractions.IdentityTokenService.IdentityTokenServiceBase
    {
        private readonly UserManager<User> _userManager;
        private readonly Data.Stores.UserStore _userStore;
        
        public IdentityTokenService(UserManager<User> userManager, Data.Stores.UserStore userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
        }
        
        public override async Task<IdentityTokenServiceResponse> GetToken(IdentityTokenServiceRequest request, ServerCallContext context)
        {
            var user = await _userManager.FindByNameAsync(request?.UserId.ToString());
            if (user == null)
                return new IdentityTokenServiceResponse();

            return new IdentityTokenServiceResponse
            {
                Token = await _userStore.GetTokenAsync(user, "Vkontakte", "access_token_implicit", context.CancellationToken) 
            };
        }
    }
}