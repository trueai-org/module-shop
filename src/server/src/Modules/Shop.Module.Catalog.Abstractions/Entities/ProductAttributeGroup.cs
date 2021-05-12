using Shop.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品属性组（鞋子属性、运动鞋属性、女装属性、男装属性、公共属性、关键属性、首饰属性、电子产品属性等）
    /// </summary>
    public class ProductAttributeGroup : EntityBase
    {
        public ProductAttributeGroup()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public IList<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
