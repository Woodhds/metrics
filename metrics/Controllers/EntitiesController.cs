using DAL;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntitiesController<T> : ControllerBase where T: BaseEntity
    {
        private readonly IRepository<T> _repository;
        public EntitiesController(IRepository<T> repository) 
        {
            _repository = repository;
        }
    }
}