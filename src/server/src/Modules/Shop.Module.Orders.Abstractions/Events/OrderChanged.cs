using MediatR;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Models;

namespace Shop.Module.Orders.Events
{
    public class OrderChanged : INotification
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public OrderStatus? OldStatus { get; set; }

        public OrderStatus NewStatus { get; set; }

        public int UserId { get; set; }

        public string Note { get; set; }
    }
}
