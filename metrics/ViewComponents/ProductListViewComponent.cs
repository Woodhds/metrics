using DAL;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace metrics.ViewComponents
{
    [ViewComponent(Name = "ProductList")]
    public class ProductListViewComponent : ViewComponent
    {
        private readonly IRepository<Product> _repository;
        public ProductListViewComponent(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? categoryId)
        {
            if (categoryId.HasValue)
            {
                return View(await _repository.Read()
                    .Include(z => z.ProductImages)
                    .Include(c => c.ProductCategory)
                    .Where(c => c.ProductCategory.Id == categoryId).ToListAsync());
            }

            return View(await _repository.Read()
                .Include(z => z.ProductImages).Include(c => c.ProductCategory)
                .ToListAsync());
        }
    }
}