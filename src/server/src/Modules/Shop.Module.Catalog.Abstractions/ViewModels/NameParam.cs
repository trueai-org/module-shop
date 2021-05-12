using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.ViewModels
{
    public class NameParam
    {
        [Required]
        public string Name { get; set; }
    }
}
