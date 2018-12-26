using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
