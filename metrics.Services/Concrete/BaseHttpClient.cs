using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using metrics.Services.Abstract;

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
                var builder = new UriBuilder(url);
                if(@params != null)
                {
                    var query = string.Join("&", Enumerable.Range(0, @params.Count).Select(c => $"{@params.GetKey(c)}={@params.Get(c)}"));
                    builder.Query = query;

                }
                var response = await _httpClient.GetAsync(builder.ToString());
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
