using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Controllers
{

    [Authorize(Roles = "admin")]
    [Route("api/widget-carousels")]
    public class WidgetCarouselApiController : ControllerBase
    {
        private readonly IRepository<WidgetInstance> _widgetInstanceRepository;
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _mediaRepository;

        public WidgetCarouselApiController(
            IRepository<WidgetInstance> widgetInstanceRepository,
            IMediaService mediaService,
            IRepository<Media> mediaRepository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
            _mediaService = mediaService;
            _mediaRepository = mediaRepository;
        }

        [HttpGet("{id}")]
        public async Task<Result> Get(long id)
        {
            var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (widgetInstance == null)
                return Result.Fail("单据不存在");
            var model = new WidgetCarouselResult
            {
                Id = widgetInstance.Id,
                Name = widgetInstance.Name,
                WidgetZoneId = widgetInstance.WidgetZoneId,
                PublishStart = widgetInstance.PublishStart,
                PublishEnd = widgetInstance.PublishEnd,
                DisplayOrder = widgetInstance.DisplayOrder,
                Items = JsonConvert.DeserializeObject<IList<WidgetCarouselItem>>(widgetInstance.Data)
            };
            if (model.Items == null)
                model.Items = new List<WidgetCarouselItem>();
            if (model.Items.Count > 0)
            {
                var mediaIds = model.Items.Select(c => c.ImageId).Distinct();
                var medias = await _mediaRepository.Query().Where(c => mediaIds.Contains(c.Id)).ToListAsync();
                foreach (var item in model.Items)
                {
                    item.ImageUrl = medias.FirstOrDefault(c => c.Id == item.ImageId)?.Url;
                }
            }
            return Result.Ok(model);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody]WidgetCarouselParam model)
        {
            var widgetInstance = new WidgetInstance
            {
                Name = model.Name,
                WidgetId = (int)WidgetWithId.CarouselWidget,
                WidgetZoneId = model.WidgetZoneId,
                PublishStart = model.PublishStart,
                PublishEnd = model.PublishEnd,
                DisplayOrder = model.DisplayOrder,
                Data = JsonConvert.SerializeObject(model.Items)
            };
            _widgetInstanceRepository.Add(widgetInstance);
            await _widgetInstanceRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody]WidgetCarouselParam model)
        {
            var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (widgetInstance == null)
                return Result.Fail("单据不存在");
            widgetInstance.Name = model.Name;
            widgetInstance.PublishStart = model.PublishStart;
            widgetInstance.PublishEnd = model.PublishEnd;
            widgetInstance.WidgetZoneId = model.WidgetZoneId;
            widgetInstance.DisplayOrder = model.DisplayOrder;
            widgetInstance.Data = JsonConvert.SerializeObject(model.Items);
            widgetInstance.UpdatedOn = DateTime.Now;
            await _widgetInstanceRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
