using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using metrics.Services.Abstract;
using metrics.Services.Helpers;

namespace metrics.Services
{
    public class BaseHttpClient: IBaseHttpClient
    {
        protected readonly HttpClient _httpClient;
        public BaseHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        async Task<T> IBaseHttpClient.PostAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            throw new NotImplementedException();
        }

        async Task<T> IBaseHttpClient.GetAsync<T>(string url, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }

                return default(T);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}
