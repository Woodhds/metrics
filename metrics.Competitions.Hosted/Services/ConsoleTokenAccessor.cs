using System.Threading.Tasks;
using Base.Contracts.Options;
using metrics.Authentication;
using metrics.Authentication.Infrastructure;
using Microsoft.Extensions.Options;

namespace metrics.Competitions.Hosted.Services
{
    public class ConsoleTokenAccessor : IUserTokenAccessor
    {
        private readonly IOptions<TokenOptions> _options;
        
        public ConsoleTokenAccessor(IOptions<TokenOptions> options)
        {
            _options = options;
        }
        public Task<string> GetTokenAsync()
        {
            return Task.FromResult(_options?.Value?.Value ?? "");
        }
    }
}