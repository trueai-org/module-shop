using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/widget-simple-products")]
    public class WidgetSimpleProductApiController : ControllerBase
    {
        private readonly IRepository<WidgetInstance> _widgetInstanceRepository;
        private readonly IRepository<Product> _productRepository;

        public WidgetSimpleProductApiController(
            IRepository<WidgetInstance> widgetInstanceRepository,
            IRepository<Product> productRepository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
            _productRepository = productRepository;
        }

        [HttpGet("{id}")]
        public async Task<Result> Get(int id)
        {
            var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (widgetInstance == null)
                return Result.Fail("单据不存在");
            var model = new WidgetSimpleProductResult
            {
                Id = widgetInstance.Id,
                Name = widgetInstance.Name,
                WidgetZoneId = widgetInstance.WidgetZoneId,
                PublishStart = widgetInstance.PublishStart,
                PublishEnd = widgetInstance.PublishEnd,
                DisplayOrder = widgetInstance.DisplayOrder,
                Setting = JsonConvert.DeserializeObject<WidgetSimpleProductSetting>(widgetInstance.Data)
            };
            if (model.Setting == null)
            {
                model.Setting = new WidgetSimpleProductSetting();
            }
            if (model.Setting?.Products?.Count > 0)
            {
                // 验证发布状态
                var productIds = model.Setting.Products.Select(c => c.Id).Distinct();
                model.Setting.Products = await _productRepository.Query().Where(c => productIds.Contains(c.Id)).Select(c => new ProductLinkResult()
                {
                    Id = c.Id,
                    IsPublished = c.IsPublished,
                    Name = c.Name
                }).ToListAsync();
            }
            return Result.Ok(model);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody] WidgetSimpleProductParam model)
        {
            var widgetInstance = new WidgetInstance
            {
                Name = model.Name,
                WidgetId = (int)WidgetWithId.SimpleProductWidget,
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

        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody]WidgetSimpleProductParam model)
        {
            var widgetInstance = _widgetInstanceRepository.Query().FirstOrDefault(x => x.Id == id);
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
