using System.Threading.Tasks;

namespace metrics.Identity.Client.Abstractions
{
    public interface IIdentityClient
    {
        Task<string> GetToken(int userId);
    }
}