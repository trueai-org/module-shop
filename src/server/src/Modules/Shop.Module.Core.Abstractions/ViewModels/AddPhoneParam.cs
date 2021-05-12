using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class AddPhoneParam
    {
        [Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "手机号格式错误")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "请输入验证码")]
        public string Code { get; set; }
    }
}
