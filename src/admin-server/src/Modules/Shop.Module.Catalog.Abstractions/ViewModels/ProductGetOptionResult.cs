using Shop.Module.Catalog.Abstractions.Models;
using System.Collections.Generic;

namespace Shop.Module.Catalog.Abstractions.ViewModels
{
    public class ProductGetOptionResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public OptionDisplayType DisplayType { get; set; }

        public IList<ProductGetOptionValueResult> Values { get; set; } = new List<ProductGetOptionValueResult>();
    }
}
