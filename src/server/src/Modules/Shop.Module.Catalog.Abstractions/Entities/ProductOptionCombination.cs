using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品属性组合
    /// </summary>
    public class ProductOptionCombination : EntityBase
    {
        public ProductOptionCombination()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int OptionId { get; set; }

        public ProductOption Option { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(450)]
        public string Value { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
