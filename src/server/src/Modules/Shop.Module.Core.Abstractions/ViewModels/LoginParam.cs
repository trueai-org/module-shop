using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class LoginParam
    {
        [Required(ErrorMessage = "请输入用户名/邮箱/手机号码")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
