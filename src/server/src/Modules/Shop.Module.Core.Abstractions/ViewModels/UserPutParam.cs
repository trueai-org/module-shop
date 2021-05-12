using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class UserPutParam
    {
        /// <summary>
        /// 昵称/全名
        /// </summary>
        [Required(ErrorMessage = "请输入您的昵称/全名")]
        [StringLength(20, ErrorMessage = "昵称不能超过20个字符")]
        public string FullName { get; set; }

        public int? MediaId { get; set; }

        //public string AdminRemark { get; set; }
    }
}
