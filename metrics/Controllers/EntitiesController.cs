using System.Linq;
using System.Threading.Tasks;
using DAL;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    public class EntitiesController<T> : Controller where T: BaseEntity
    {
        private readonly IRepository<T> _repository;
        public EntitiesController(IRepository<T> repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<DataSourceResult> Get(DataSourceRequest request)
        {
            return await Kendo.Mvc.Extensions.QueryableExtensions.ToDataSourceResultAsync(_repository.Read(), request);
        }
    }
}