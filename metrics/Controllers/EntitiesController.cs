using DAL;
using DAL.Models;
using DAL.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using metrics.Models;
using System.Threading.Tasks;
using DAL.Extensions;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [GenericControllerNameConvention]
    public class EntitiesController<T> : ControllerBase where T: BaseEntity
    {
        private readonly IRepository<T> _repository;
        private readonly IViewConfigService _viewConfigService;
        public EntitiesController(IRepository<T> repository, IViewConfigService viewConfigService) 
        {
            _repository = repository;
            _viewConfigService = viewConfigService;
        }

        [HttpGet("")]
        public async Task<ActionResult<DataSourceResponseModel>> Read(int page, int pageSize, string[] columns)
        {
            var query = _repository.Read().OrderByDescending(c => c.Id).Select(columns);
            var count = await query.CountAsync();
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToDynamicList();
            return Ok(new DataSourceResponseModel(data, count));
        }

        [HttpGet("config")]
        public ActionResult<ViewConfig> GetConfig() 
        {
            return Ok(_viewConfigService.GetConfig<T>());
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<T>> Filter(string startWith)
        {
            var lookup = _viewConfigService.GetConfig<T>()?.LookupProperty;
            return Ok(_repository.Read().Where($"{lookup}.StartWith(@0)", lookup));

        }

        [HttpPost("")]
        public async Task<ActionResult<bool>> Save(T model)
        {
            if (model.Id != 0)
            {
                await _repository.CreateAsync(model);
            }
            else
            {
                await _repository.UpdateAsync(model);
            }
            return Ok(true);
        }
    }
}