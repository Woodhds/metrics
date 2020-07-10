using System.Threading.Tasks;

namespace metrics.Authentication.Infrastructure
{
    public interface IUserTokenAccessor
    {
        Task<string> GetTokenAsync();
    }
}