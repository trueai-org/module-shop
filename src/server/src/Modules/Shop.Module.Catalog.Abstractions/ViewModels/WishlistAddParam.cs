using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.ViewModels
{
    public class WishlistAddParam
    {
        [Required]
        public int ProductId { get; set; }
    }
}
