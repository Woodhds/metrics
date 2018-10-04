using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index(string slug)
        {
            return
            View();
        }
    }
}