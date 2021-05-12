using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductQueryParam
    {
        public string Name { get; set; }

        public bool? HasOptions { get; set; }

        /// <summary>
        /// Gets or sets the values indicating whether this product is visible in catalog or search results.
        /// It's used when this product is associated to some "grouped" one
        /// This way associated products could be accessed/added/etc only from a grouped product details page
        /// </summary>
        public bool? IsVisibleIndividually { get; set; }

        public bool? IsFeatured { get; set; }

        public bool? IsAllowToOrder { get; set; }

        public bool? IsPublished { get; set; }

        public string Sku { get; set; }

        public IList<int> CategoryIds { get; set; } = new List<int>();

        public bool IncludeSubCategories { get; set; }
    }
}
