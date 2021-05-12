using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 品牌
    /// </summary>
    public class Brand : EntityBase
    {
        public Brand()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        [Required]
        [StringLength(450)]
        public string Slug { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
