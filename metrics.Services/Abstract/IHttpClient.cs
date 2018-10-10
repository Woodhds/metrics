using System.Collections.Specialized;
using System.Threading.Tasks;

namespace metrics.Services.Abstract
{
    public interface IHttpClient
    {
        Task<T> GetAsync<T>(string url, NameValueCollection @params = null);
    }
}
