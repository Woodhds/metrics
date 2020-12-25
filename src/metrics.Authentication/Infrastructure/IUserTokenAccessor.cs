using System.Threading.Tasks;

namespace metrics.Authentication.Infrastructure
{
    public interface IUserTokenAccessor
    {
        ValueTask<string> GetTokenAsync();
    }
}