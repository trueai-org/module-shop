using System;

namespace Shop.Module.Catalog.ViewModels
{
    public class BrandResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsPublished { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
