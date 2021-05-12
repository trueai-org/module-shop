using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品属性数据（季节：春季、夏季、秋季、冬季）
    /// </summary>
    public class ProductAttributeData : EntityBase
    {
        public int AttributeId { get; set; }

        public ProductAttribute Attribute { get; set; }

        [StringLength(450)]
        [Required]
        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
