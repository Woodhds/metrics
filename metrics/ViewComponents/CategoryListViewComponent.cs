using DAL;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace metrics.ViewComponents
{
    [ViewComponent(Name = "CategoryList")]
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly IRepository<ProductCategory> _repository;
        public CategoryListViewComponent(IRepository<ProductCategory> repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? activeId)
        {
            if(activeId.HasValue)
            {
                ViewBag.ActiveId = activeId.Value;
            }
            return View(await _repository.Read().ToListAsync());
        }
    }
}
