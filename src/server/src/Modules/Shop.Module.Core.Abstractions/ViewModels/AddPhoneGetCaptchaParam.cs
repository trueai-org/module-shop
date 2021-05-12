using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class AddPhoneGetCaptchaParam
    {
        [Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "手机号格式错误")]
        public string Phone { get; set; }
    }
}
