using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Orders.Models;
using System;

namespace Shop.Module.Orders.Entities
{
    public class OrderHistory : EntityBase
    {
        public OrderHistory()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public OrderStatus? OldStatus { get; set; }

        public OrderStatus NewStatus { get; set; }

        public string OrderSnapshot { get; set; }

        public string Note { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public int UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
