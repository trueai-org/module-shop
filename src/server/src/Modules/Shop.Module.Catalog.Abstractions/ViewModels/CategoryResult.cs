using System;

namespace Shop.Module.Catalog.ViewModels
{
    public class CategoryResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public bool IncludeInMenu { get; set; }

        public bool IsPublished { get; set; }

        public int? ParentId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
