using MediatR;
using Shop.Module.Orders.Entities;

namespace Shop.Module.Orders.Events
{
    public class OrderCreated : INotification
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int UserId { get; set; }

        public string Note { get; set; }
    }
}
