using MediatR;

namespace Shop.Module.Core.Abstractions.Events
{
    public class EntityDeleting : INotification
    {
        public int EntityId { get; set; }
    }
}
