using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class ConfirmEmailParam
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
