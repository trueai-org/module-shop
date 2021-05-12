using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class LoginPhoneGetCaptchaParam
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "手机号格式错误")]
        public string Phone { get; set; }
    }
}
