using System;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    [Route("api/[controller]")]
    public class SiteController : ControllerBase
    {

        private readonly ICompetitionsService _competitionsService;

        public SiteController(ICompetitionsService competitionsService)
        {
            _competitionsService = competitionsService;
        }

        public async Task<ActionResult<DataSourceResponseModel>> Get(int page = 0)
        {
            try
            {
                var data = await _competitionsService.Fetch(page);
                return new DataSourceResponseModel(data, data.Count);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}