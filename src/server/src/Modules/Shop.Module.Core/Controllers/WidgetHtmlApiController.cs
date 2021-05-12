using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.ViewModels;
using System;
using System.Threading.Tasks;

namespace Shop.Module.Core.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/widget-html")]
    public class WidgetHtmlApiController : ControllerBase
    {
        private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

        public WidgetHtmlApiController(IRepository<WidgetInstance> widgetInstanceRepository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
        }

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

        [HttpPost]
        public async Task<Result> Post([FromBody]WidgetHtmlParam model)
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

        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody]WidgetHtmlParam model)
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
