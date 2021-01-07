using System.Threading.Tasks;
using Grpc.Core;
using metrics.Identity.Client.Abstractions;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Services;
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
        private readonly IEncryptionService _encryptionService;
        
        public IdentityTokenService(UserManager<User> userManager, Data.Stores.UserStore userStore, IEncryptionService encryptionService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _encryptionService = encryptionService;
        }
        
        public override async Task<IdentityTokenServiceResponse> GetToken(IdentityTokenServiceRequest request, ServerCallContext context)
        {
            var user = await _userManager.FindByNameAsync(request?.UserId.ToString());
            if (user == null)
                return new IdentityTokenServiceResponse();

            return new IdentityTokenServiceResponse
            {
                Token =_encryptionService.Decrypt(await _userStore.GetTokenAsync(user, "Vkontakte", "access_token_implicit", context.CancellationToken)) 
            };
        }
    }
}