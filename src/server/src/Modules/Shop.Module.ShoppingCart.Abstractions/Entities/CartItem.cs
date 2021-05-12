using Shop.Infrastructure.Models;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using System;

namespace Shop.Module.ShoppingCart.Entities
{
    public class CartItem : EntityBase
    {
        public CartItem()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            IsChecked = true;
        }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; }

        public bool IsChecked { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public int UpdatedById { get; set; }

        public User UpdatedBy { get; set; }
    }
}
