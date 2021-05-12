using Shop.Infrastructure.Models;
using Shop.Module.Catalog.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 产品选项（销售属性，尺码、颜色等）
    /// </summary>
    public class ProductOption : EntityBase
    {
        public ProductOption()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public ProductOption(int id) : this()
        {
            Id = id;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public OptionDisplayType DisplayType { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
