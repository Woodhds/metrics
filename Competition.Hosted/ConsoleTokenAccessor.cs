using metrics.Services.Abstract;
using metrics.Services.Options;
using Microsoft.Extensions.Options;

namespace Competition.Hosted
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