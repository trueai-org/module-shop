using MediatR;
using Shop.Module.Orders.Abstractions.Entities;

namespace Shop.Module.Orders.Abstractions.Events
{
    public class OrderCreated : INotification
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int UserId { get; set; }

        public string Note { get; set; }
    }
}
