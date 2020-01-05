using metrics.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppController : ControllerBase
    {
        private readonly VkontakteOptions _options;

        public AppController(IOptions<VkontakteOptions> options)
        {
            _options = options.Value;
        }

        public ActionResult<VkontakteOptions> Get()
        {
            return _options;
        }
    }
}