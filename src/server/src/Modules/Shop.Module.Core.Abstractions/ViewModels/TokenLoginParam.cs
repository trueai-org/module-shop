using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class TokenLoginParam
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
