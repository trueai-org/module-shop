using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    public class ProductOptionValue : EntityBase
    {
        public ProductOptionValue()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int OptionId { get; set; }

        public ProductOption Option { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        [StringLength(450)]
        public string Value { get; set; }

        [StringLength(450)]
        public string Display { get; set; }

        public int DisplayOrder { get; set; }

        public int? MediaId { get; set; }

        public Media Media { get; set; }

        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
