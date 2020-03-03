using Base.Contracts.Options;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace metrics.Services.Utils
{
    public class ConsoleTokenAccessor : IVkTokenAccessor
    {
        private readonly IOptions<TokenOptions> _options;
        public ConsoleTokenAccessor(IOptions<TokenOptions> options)
        {
            _options = options;
        }
        public string GetToken()
        {
            return _options?.Value?.Value ?? "";
        }
    }
}