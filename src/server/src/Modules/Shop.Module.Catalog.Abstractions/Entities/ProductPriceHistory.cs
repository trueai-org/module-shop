using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;

namespace Shop.Module.Catalog.Entities
{
    public class ProductPriceHistory : EntityBase
    {
        public ProductPriceHistory()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public Product Product { get; set; }

        public decimal? Price { get; set; }

        public decimal? OldPrice { get; set; }

        public decimal? SpecialPrice { get; set; }

        public DateTime? SpecialPriceStart { get; set; }

        public DateTime? SpecialPriceEnd { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public int UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
