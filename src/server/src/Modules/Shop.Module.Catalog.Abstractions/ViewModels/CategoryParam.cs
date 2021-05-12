using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.ViewModels
{
    public class CategoryParam
    {
        public CategoryParam()
        {
            IsPublished = true;
        }

        public int Id { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string MetaTitle { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public int DisplayOrder { get; set; }

        public int? ParentId { get; set; }

        public bool IncludeInMenu { get; set; }

        public bool IsPublished { get; set; }

        public int? MediaId { get; set; }

        public string ThumbnailImageUrl { get; set; }
    }
}
