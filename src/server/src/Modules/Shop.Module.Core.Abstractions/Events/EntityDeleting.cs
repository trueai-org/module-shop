using MediatR;

namespace Shop.Module.Core.Events
{
    public class EntityDeleting : INotification
    {
        public int EntityId { get; set; }
    }
}
