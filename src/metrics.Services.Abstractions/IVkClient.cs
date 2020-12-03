using System.Net.Http;
using System.Threading.Tasks;

namespace metrics.Services.Abstractions
{
    public interface IVkClient
    {
        Task<string> GetStringAsync(string requestUri);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }
}