using Shop.Module.Catalog.Models;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductOptionResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public OptionDisplayType DisplayType { get; set; }
    }
}
