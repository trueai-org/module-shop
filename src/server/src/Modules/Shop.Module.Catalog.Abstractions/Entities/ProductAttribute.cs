using Shop.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品属性
    /// </summary>
    public class ProductAttribute : EntityBase
    {
        public ProductAttribute()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public int GroupId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public ProductAttributeGroup Group { get; set; }

        public IList<ProductAttributeTemplateRelation> ProductTemplates { get; protected set; } = new List<ProductAttributeTemplateRelation>();
    }
}
