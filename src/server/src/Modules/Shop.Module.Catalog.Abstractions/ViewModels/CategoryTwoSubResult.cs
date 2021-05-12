using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class CategoryTwoSubResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public int DisplayOrder { get; set; }

        public int? ParentId { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public IList<CategoryTwoSubResult> SubCategories { get; set; } = new List<CategoryTwoSubResult>();
    }
}
