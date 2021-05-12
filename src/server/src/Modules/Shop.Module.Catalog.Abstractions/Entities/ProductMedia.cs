using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;

namespace Shop.Module.Catalog.Entities
{
    public class ProductMedia : EntityBase
    {
        public ProductMedia()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int MediaId { get; set; }

        public Media Media { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
