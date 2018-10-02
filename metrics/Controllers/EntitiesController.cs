using System.Linq;
using System.Threading.Tasks;
using DAL;
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
        public async Task<IQueryable<T>> Get()
        {
            return _repository.Read();
        }
    }
}