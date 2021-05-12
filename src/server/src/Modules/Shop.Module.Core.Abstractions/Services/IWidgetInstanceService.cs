using Shop.Module.Core.Entities;
using System.Linq;

namespace Shop.Module.Core.Services
{
    public interface IWidgetInstanceService
    {
        IQueryable<WidgetInstance> GetPublished();
    }
}
