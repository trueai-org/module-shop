using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.SampleData.Services;
using Shop.Module.SampleData.ViewModels;

namespace Shop.Module.SampleData.Controllers
{
    /// <summary>
    /// 示例数据控制器，用于管理和操作示例数据的重置和生成。
    /// </summary>
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

        /// <summary>
        /// 重置应用数据为示例数据。该操作会将当前数据清空，并导入预定义的示例数据。
        /// </summary>
        /// <param name="model">包含示例数据重置选项的模型。</param>
        /// <returns>返回操作结果。成功时返回成功状态码，失败时返回错误信息。</returns>
        [HttpPost]
        public async Task<Result> ResetToSample([FromBody] SampleDataOption model)
        {
            await _sampleDataService.ResetToSampleData(model);
            return Result.Ok();
        }

        /// <summary>
        /// 生成省/市/区数据。该操作通常用于初始化地址相关的数据表。
        /// </summary>
        /// <returns>返回操作结果。成功时返回成功状态码，失败时返回错误信息。</returns>
        [HttpPost("provinces")]
        public async Task<Result> GenPcas()
        {
            await _stateOrProvinceService.GenPcas();
            return Result.Ok();
        }
    }
}