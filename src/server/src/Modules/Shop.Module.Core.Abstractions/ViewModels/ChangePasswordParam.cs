using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class ChangePasswordParam
    {
        [Required(ErrorMessage = "请输入旧密码")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "请输入新密码")]
        [StringLength(100, ErrorMessage = "密码长度6-32字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不匹配")]
        public string ConfirmPassword { get; set; }
    }
}
