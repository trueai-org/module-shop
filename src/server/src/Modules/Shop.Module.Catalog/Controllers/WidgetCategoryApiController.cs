using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 管理后台控制器用于处理小部件类别相关操作的 API 请求。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/widget-categories")]
    public class WidgetCategoryApiController : ControllerBase
    {
        private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

        public WidgetCategoryApiController(IRepository<WidgetInstance> widgetInstanceRepository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
        }


        /// <summary>
        /// 根据指定的小部件实例 ID 获取小部件类别信息。
        /// </summary>
        /// <param name="id">小部件实例 ID。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpGet("{id}")]
        public async Task<Result> Get(int id)
        {
            var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(c => c.Id == id);
            if (widgetInstance == null)
                return Result.Fail("单据不存在");
            var model = new WidgetCategoryResult
            {
                Id = widgetInstance.Id,
                Name = widgetInstance.Name,
                WidgetZoneId = widgetInstance.WidgetZoneId,
                PublishStart = widgetInstance.PublishStart,
                PublishEnd = widgetInstance.PublishEnd,
                DisplayOrder = widgetInstance.DisplayOrder,
                Setting = JsonConvert.DeserializeObject<WidgetCategorySetting>(widgetInstance.Data ?? JsonConvert.SerializeObject(new WidgetCategorySetting()))
            };
            return Result.Ok(model);
        }

        /// <summary>
        /// 创建一个新的小部件类别。
        /// </summary>
        /// <param name="model">要创建的小部件类别参数。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] WidgetCategoryParam model)
        {
            var widgetInstance = new WidgetInstance
            {
                Name = model.Name,
                WidgetId = (int)WidgetWithId.CategoryWidget,
                WidgetZoneId = model.WidgetZoneId,
                PublishStart = model.PublishStart,
                PublishEnd = model.PublishEnd,
                DisplayOrder = model.DisplayOrder,
                Data = JsonConvert.SerializeObject(model.Setting)
            };
            _widgetInstanceRepository.Add(widgetInstance);
            await _widgetInstanceRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定 ID 的小部件类别信息。
        /// </summary>
        /// <param name="id">小部件实例 ID。</param>
        /// <param name="model">更新后的小部件类别参数。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody] WidgetCategoryParam model)
        {
            var widgetInstance = await _widgetInstanceRepository.FirstOrDefaultAsync(id);
            if (widgetInstance == null)
                return Result.Fail("单据不存在");
            widgetInstance.Name = model.Name;
            widgetInstance.WidgetZoneId = model.WidgetZoneId;
            widgetInstance.PublishStart = model.PublishStart;
            widgetInstance.PublishEnd = model.PublishEnd;
            widgetInstance.DisplayOrder = model.DisplayOrder;
            widgetInstance.Data = JsonConvert.SerializeObject(model.Setting);
            widgetInstance.UpdatedOn = DateTime.Now;
            await _widgetInstanceRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}