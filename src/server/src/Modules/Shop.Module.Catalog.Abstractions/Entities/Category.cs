using Newtonsoft.Json;
using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities
{
    /// <summary>
    /// 分类
    /// </summary>
    public class Category : EntityBase
    {
        public Category()
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

        public string MetaTitle { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }

        public bool IncludeInMenu { get; set; }

        public int? ParentId { get; set; }

        public Category Parent { get; set; }

        public int? MediaId { get; set; }

        public Media Media { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        [JsonIgnore]
        public IList<Category> Children { get; protected set; } = new List<Category>();
    }
}
