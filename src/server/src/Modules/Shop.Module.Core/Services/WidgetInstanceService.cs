using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Services;
using System;
using System.Linq;

namespace Shop.Module.Core.Services
{
    public class WidgetInstanceService : IWidgetInstanceService
    {
        private IRepository<WidgetInstance> _widgetInstanceRepository;

        public WidgetInstanceService(IRepository<WidgetInstance> widgetInstanceRepository)
        {
            _widgetInstanceRepository = widgetInstanceRepository;
        }

        public IQueryable<WidgetInstance> GetPublished()
        {
            return _widgetInstanceRepository.Query().Where(x =>
            x.PublishStart.HasValue && x.PublishStart < DateTime.Now
            && (!x.PublishEnd.HasValue || x.PublishEnd > DateTime.Now));
        }
    }
}
