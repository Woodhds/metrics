using System.Threading;
using System.Threading.Tasks;
using metrics.Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace metrics.Identity.Data.Stores
{
    public class UserStore : UserStore<User, Role, IdentityContext, int, UserClaim, UserRole, UserLogin, UserToken,
        RoleClaim>
    {
        public UserStore(IdentityContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public override Task SetTokenAsync(User user, string loginProvider, string name, string value,
            CancellationToken cancellationToken)
        {
            base.SetTokenAsync(user, loginProvider, name, value, cancellationToken);
            return SaveChanges(cancellationToken);

        }
    }
}