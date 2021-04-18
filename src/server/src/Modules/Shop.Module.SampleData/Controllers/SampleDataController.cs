using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.SampleData.Services;
using Shop.Module.SampleData.ViewModels;
using System.Threading.Tasks;

namespace Shop.Module.SampleData.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/sample-data")]
    public class SampleDataController : ControllerBase
    {
        private readonly ISampleDataService _sampleDataService;
        private readonly IStateOrProvinceService _stateOrProvinceService;

        public SampleDataController(
            ISampleDataService sampleDataService,
            IStateOrProvinceService stateOrProvinceService)
        {
            _sampleDataService = sampleDataService;
            _stateOrProvinceService = stateOrProvinceService;
        }

        [HttpPost]
        public async Task<Result> ResetToSample([FromBody]SampleDataOption model)
        {
            await _sampleDataService.ResetToSampleData(model);
            return Result.Ok();
        }

        [HttpPost("provinces")]
        public async Task<Result> GenPcas()
        {
            await _stateOrProvinceService.GenPcas();
            return Result.Ok();
        }
    }
}
