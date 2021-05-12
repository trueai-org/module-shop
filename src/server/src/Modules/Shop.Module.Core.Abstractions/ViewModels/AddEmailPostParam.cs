using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class AddEmailPostParam
    {
        [Required]
        [EmailAddress(ErrorMessage = "邮箱地址格式错误")]
        public string Email { get; set; }
    }
}
