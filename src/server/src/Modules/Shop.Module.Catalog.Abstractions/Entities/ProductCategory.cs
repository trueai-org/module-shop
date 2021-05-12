using Shop.Infrastructure.Models;
using System;

namespace Shop.Module.Catalog.Entities
{
    public class ProductCategory : EntityBase
    {
        public ProductCategory()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int CategoryId { get; set; }

        public int ProductId { get; set; }

        public Category Category { get; set; }

        public Product Product { get; set; }

        public bool IsFeaturedProduct { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
