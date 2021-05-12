using Shop.Infrastructure.Models;
using System;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品属性模板与产品属性关联表（多对多关系）
    /// </summary>
    public class ProductAttributeTemplateRelation : EntityBase
    {
        public ProductAttributeTemplateRelation()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }
        public int TemplateId { get; set; }

        public ProductAttributeTemplate Template { get; set; }

        public int AttributeId { get; set; }

        public ProductAttribute Attribute { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
