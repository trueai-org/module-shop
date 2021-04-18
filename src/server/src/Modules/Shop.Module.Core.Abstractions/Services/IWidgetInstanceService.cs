using Shop.Module.Core.Abstractions.Entities;
using System.Linq;

namespace Shop.Module.Core.Abstractions.Services
{
    public interface IWidgetInstanceService
    {
        IQueryable<WidgetInstance> GetPublished();
    }
}
