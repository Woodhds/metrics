using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using metrics.Services.Helpers;
using metrics.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace metrics.Services
{
    public class BaseHttpClient : IBaseHttpClient
    {
        protected readonly HttpClient _httpClient;
        public BaseHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        protected Task<T> PostAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            throw new NotImplementedException();
        }

        protected async Task<T> GetAsync<T>(string url, NameValueCollection @params = null)
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
