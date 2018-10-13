using System.Collections.Specialized;
using System.Threading.Tasks;

namespace metrics.Services.Abstract
{
    public interface IBaseHttpClient
    {
        Task<T> GetAsync<T>(string url, NameValueCollection @params = null);
        Task<T> PostAsync<T>(string url, object content, NameValueCollection @params = null);
    }
}
