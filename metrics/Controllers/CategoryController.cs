using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace metrics.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IRepository<ProductCategory> _repository;

        public CategoryController(IRepository<ProductCategory> repository)
        {
            _repository = repository;
        }
        
        public async Task<IActionResult> Index(string slug)
        {
            if (string.IsNullOrEmpty(slug?.Trim()))
            {
                return NotFound();
            }

            var slugNormalized = slug.Trim().ToUpper();
            var category = await _repository.Read().Where(c => c.Slug.ToUpper() == slugNormalized)
                .SingleOrDefaultAsync();
            if (category == null)
                return NotFound();
            return View(category.Id);
        }
    }
}