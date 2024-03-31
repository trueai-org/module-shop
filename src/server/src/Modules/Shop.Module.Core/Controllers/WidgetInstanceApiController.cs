using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 小部件实例 API 控制器，提供部件实例相关的接口操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/widget-instances")]
    public class WidgetInstanceApiController : ControllerBase
    {
        private readonly IRepository<WidgetInstance> _widgetInstanceRepository;
        private readonly IRepository<Widget> _widgetRespository;

        public WidgetInstanceApiController(
            IRepository<WidgetInstance> widgetInstanceRepository,
            IRepository<Widget> widgetRespository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
            _widgetRespository = widgetRespository;
        }

        /// <summary>
        /// 获取所有部件实例。
        /// </summary>
        /// <returns>所有部件实例的信息。</returns>
        [HttpGet]
        public async Task<Result> Get()
        {
            var widgetInstances = await _widgetInstanceRepository.Query()
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    WidgetType = x.Widget.Name,
                    x.WidgetZoneId,
                    x.WidgetId,
                    WidgetZone = x.WidgetZone.Name,
                    CreatedOn = x.CreatedOn,
                    EditUrl = x.Widget.EditUrl,
                    PublishStart = x.PublishStart,
                    PublishEnd = x.PublishEnd,
                    DisplayOrder = x.DisplayOrder,
                }).ToListAsync();

            return Result.Ok(widgetInstances.OrderBy(c => c.WidgetZoneId).ThenBy(c => c.DisplayOrder));
        }

        /// <summary>
        /// 根据部件实例ID删除部件实例。
        /// </summary>
        /// <param name="id">部件实例ID。</param>
        /// <returns>操作结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (widgetInstance != null)
            {
                widgetInstance.IsDeleted = true;
                widgetInstance.UpdatedOn = DateTime.Now;
                _widgetInstanceRepository.SaveChanges();
            }
            return Result.Ok();
        }

        /// <summary>
        /// 获取部件实例数量。
        /// </summary>
        /// <returns>部件实例的数量。</returns>
        [HttpGet("number-of-widgets")]
        public async Task<Result> GetNumberOfWidgets()
        {
            var total = await _widgetInstanceRepository.Query().CountAsync();
            return Result.Ok(total);
        }
    }
}
