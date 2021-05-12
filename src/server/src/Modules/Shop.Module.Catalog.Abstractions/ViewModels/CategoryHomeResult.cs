using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;

namespace Shop.Module.Catalog.ViewModels
{
    public class CategoryHomeResult
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public Media ThumbnailImage { get; set; }

        public string ThumbnailUrl { get; set; }

        public static CategoryHomeResult FromCategory(Category category)
        {
            var result = new CategoryHomeResult()
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                ThumbnailImage = category.Media
            };
            return result;
        }
    }
}
