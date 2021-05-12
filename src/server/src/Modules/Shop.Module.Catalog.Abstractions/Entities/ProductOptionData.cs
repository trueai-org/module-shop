using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品选项数据（例：X、M、L、XXL、XXXL、RED、yellow、#FFFFFF、#000）
    /// </summary>
    public class ProductOptionData : EntityBase
    {
        public ProductOptionData()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int OptionId { get; set; }

        public ProductOption Option { get; set; }

        [Required]
        [StringLength(450)]
        public string Value { get; set; }

        [StringLength(450)]
        public string Display { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
