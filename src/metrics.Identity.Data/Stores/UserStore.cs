using System.Threading;
using System.Threading.Tasks;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace metrics.Identity.Data.Stores
{
    public class UserStore : UserStore<User, Role, IdentityContext, int, UserClaim, UserRole, UserLogin, UserToken,
        RoleClaim>
    {
        private readonly IEncryptionService _encryptionService;
        public UserStore(IdentityContext context, IEncryptionService encryptionService, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _encryptionService = encryptionService;
        }

        public override async Task SetTokenAsync(User user, string loginProvider, string name, string value,
            CancellationToken cancellationToken)
        {
            value = _encryptionService.Encrypt(value);
            await base.SetTokenAsync(user, loginProvider, name, value, cancellationToken);
            await SaveChanges(cancellationToken);
        }

        public override async Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
           await base.RemoveTokenAsync(user, loginProvider, name, cancellationToken);
           await SaveChanges(cancellationToken);
        }
    }
}