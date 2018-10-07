using DAL;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            if(id.HasValue)
            {
                return View(_repository.Read().Where(c => c.ProductCategory.Id == id));
            }

            return View(_repository.Read());
        }
    }
}
