using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class LoginPhoneParam
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "手机号格式错误")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "请输入验证码")]
        public string Code { get; set; }

        public bool RememberMe { get; set; }
    }
}
