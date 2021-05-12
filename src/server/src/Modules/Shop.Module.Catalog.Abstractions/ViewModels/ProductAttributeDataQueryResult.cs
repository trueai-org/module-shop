using System;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductAttributeDataQueryResult
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
