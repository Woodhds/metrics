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
    public class BaseHttpClient: IHttpClient
    {
        protected readonly HttpClient _httpClient;
        public BaseHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        protected async Task<T> GetAsync<T>(string url, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var response = await _httpClient.GetAsync(uri);
                if(response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }

                return default(T);
            }
            catch(Exception e)
            {
                return default(T);
            }
        }

        Task<T> IHttpClient.GetAsync<T>(string url, NameValueCollection @params)
        {
            throw new NotImplementedException();
        }
    }
}
