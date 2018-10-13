using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace metrics.Options
{
    public class GoogleRecaptchaService : IGoogleRecaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleRecaptcha _options;
        public GoogleRecaptchaService(IHttpClientFactory httpCientFactory, IOptions<GoogleRecaptcha> options)
        {
            _httpClient = httpCientFactory.CreateClient();
            _options = options.Value;
        }

        public async Task<bool> ValidateAsync(string userResponse)
        {
            var response = await _httpClient.PostAsync(
                        $"https://www.google.com/recaptcha/api/siteverify?secret={_options.Secret}&response={userResponse}",
                    null);
            if(response.IsSuccessStatusCode)
            {
                var googleResponse = await response.Content.ReadAsAsync<GoogleRecaptchaResponse>();
                return googleResponse.Success;
            }

            return false;
        }
    }

    public class GoogleRecaptcha
    {
        public string Secret { get; set; }
    }

    public interface IGoogleRecaptchaService
    {
        Task<bool> ValidateAsync(string userResponse);
    }

    public class GoogleRecaptchaResponse
    {
        public bool Success { get; set; }
    }
}
