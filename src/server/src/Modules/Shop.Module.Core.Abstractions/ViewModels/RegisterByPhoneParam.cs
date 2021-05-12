using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class RegisterByPhoneParam
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "手机号格式错误")]
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "请输入验证码")]
        public string Captcha { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [StringLength(100, ErrorMessage = "密码长度6-32字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "两次输入的密码不匹配")]
        public string ConfirmPassword { get; set; }

        //[EmailAddress]
        public string Email { get; set; }
    }
}
