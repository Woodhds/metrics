using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using metrics.Services.Abstract;
using metrics.Services.Helpers;

namespace metrics.Services.Concrete
{
    public class BaseHttpClient : IBaseHttpClient
    {
        private readonly HttpClient _httpClient;
        public BaseHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        protected virtual async Task<T> PostAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var response = await _httpClient.PostAsJsonAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                return await response.Content.ReadAsAsync<T>();
            }
            catch (Exception e)
            {
                return default;
            }
        }

        protected virtual async Task<T> GetAsync<T>(string url, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }

                return default;
            }
            catch (Exception e)
            {
                return default;
            }
        }
    }
}
