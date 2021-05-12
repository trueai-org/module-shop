using Shop.Module.Catalog.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductOptionParam
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public OptionDisplayType DisplayType { get; set; }
    }
}
