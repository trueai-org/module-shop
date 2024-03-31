using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 小部件 Html API 控制器，提供 HTML 部件相关的接口操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/widget-html")]
    public class WidgetHtmlApiController : ControllerBase
    {
        private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

        public WidgetHtmlApiController(IRepository<WidgetInstance> widgetInstanceRepository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
        }

        /// <summary>
        /// 根据部件实例ID获取 HTML 部件信息。
        /// </summary>
        /// <param name="id">部件实例ID。</param>
        /// <returns>HTML 部件信息。</returns>
        [HttpGet("{id}")]
        public async Task<Result> Get(int id)
        {
            var widget = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (widget == null)
            {
                return Result.Fail("单据不存在");
            }
            var model = new WidgetHtmlResult
            {
                Id = widget.Id,
                Name = widget.Name,
                WidgetZoneId = widget.WidgetZoneId,
                HtmlContent = widget.HtmlData,
                PublishStart = widget.PublishStart,
                PublishEnd = widget.PublishEnd,
                DisplayOrder = widget.DisplayOrder,
            };
            return Result.Ok(model);
        }

        /// <summary>
        /// 创建新的 HTML 部件。
        /// </summary>
        /// <param name="model">HTML 部件参数。</param>
        /// <returns>操作结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] WidgetHtmlParam model)
        {
            var widgetInstance = new WidgetInstance
            {
                Name = model.Name,
                WidgetId = (int)WidgetWithId.HtmlWidget,
                WidgetZoneId = model.WidgetZoneId,
                HtmlData = model.HtmlContent,
                PublishStart = model.PublishStart,
                PublishEnd = model.PublishEnd,
                DisplayOrder = model.DisplayOrder,
            };
            _widgetInstanceRepository.Add(widgetInstance);
            await _widgetInstanceRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新 HTML 部件信息。
        /// </summary>
        /// <param name="id">部件实例ID。</param>
        /// <param name="model">HTML 部件参数。</param>
        /// <returns>操作结果。</returns>
        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody] WidgetHtmlParam model)
        {
            var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (widgetInstance == null)
            {
                return Result.Fail("单据不存在");
            }
            widgetInstance.Name = model.Name;
            widgetInstance.WidgetZoneId = model.WidgetZoneId;
            widgetInstance.HtmlData = model.HtmlContent;
            widgetInstance.PublishStart = model.PublishStart;
            widgetInstance.PublishEnd = model.PublishEnd;
            widgetInstance.DisplayOrder = model.DisplayOrder;
            widgetInstance.UpdatedOn = DateTime.Now;
            await _widgetInstanceRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}