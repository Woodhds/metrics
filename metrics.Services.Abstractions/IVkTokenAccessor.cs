using System.Threading.Tasks;

namespace metrics.Services.Abstractions
{
    public interface IVkTokenAccessor
    {
        Task<string> GetTokenAsync(int? userId = null);
    }
}