using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class UserCreateParam
    {
        [Required]
        [RegularExpression(@"(\w[-\w.?]*@?[-\w.?]*){4,64}", ErrorMessage = "用户名长度4-64字符，只能由“字母、数字、特殊字符（._-@）”组合")]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"[0-9-()（）]{4,32}", ErrorMessage = "手机号码格式不正确")]
        public string PhoneNumber { get; set; }

        //[Required(ErrorMessage = "密码不能为空")]
        //[StringLength(maximumLength: 32, MinimumLength = 6, ErrorMessage = "密码长度6-32字符")]
        public string Password { get; set; }

        /// <summary>
        /// 已启用
        /// </summary>
        public bool IsActive { get; set; }

        public IList<int> RoleIds { get; set; } = new List<int>();

        public string AdminRemark { get; set; }
    }
}
