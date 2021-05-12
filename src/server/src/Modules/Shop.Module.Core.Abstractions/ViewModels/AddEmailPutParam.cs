using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class AddEmailPutParam
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "邮箱地址格式错误")]
        public string Email { get; set; }

        [Required()]
        public string Code { get; set; }
    }
}
