using System.Net.Http;

namespace metrics.services.tests
{
    public class TestHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}