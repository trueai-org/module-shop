using MediatR;
using Shop.Module.Core.Models;

namespace Shop.Module.Core.Events
{
    public class EntityViewed : INotification
    {
        public int EntityId { get; set; }

        public int UserId { get; set; }

        public EntityTypeWithId EntityTypeWithId { get; set; }
    }
}
