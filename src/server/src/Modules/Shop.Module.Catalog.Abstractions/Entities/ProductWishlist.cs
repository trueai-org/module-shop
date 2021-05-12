using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    public class ProductWishlist : EntityBase
    {
        public ProductWishlist()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int CustomerId { get; set; }

        public User Customer { get; set; }

        public int Quantity { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
