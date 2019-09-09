using System;
using System.Threading.Tasks;
using metrics.Models;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [Authorize(Policy = "VkPolicy")]
    public class SiteController : Controller
    {

        private readonly ICompetitionsService _competitionsService;

        public SiteController(ICompetitionsService competitionsService)
        {
            _competitionsService = competitionsService;
        }

        public ViewResult Index()
        {
            return View();
        }
        
        public async Task<ActionResult<DataSourceResponseModel>> Get()
        {
            try
            {
                var data = await _competitionsService.Fetch();
                return new DataSourceResponseModel(data, data.Count);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}