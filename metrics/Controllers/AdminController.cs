using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    public class AdminController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return
            View();
        }
    }
}