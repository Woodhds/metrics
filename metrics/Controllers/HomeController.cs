using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using metrics.Models;
using Data.Entities;
using DAL;
using Microsoft.AspNetCore.Authentication;

namespace metrics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _repository;
        public HomeController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.Read());
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
