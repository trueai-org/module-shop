using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class RefreshTokenParam
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
