using Shop.Module.Feedbacks.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Feedbacks.ViewModels
{
    public class FeedbackAddParam
    {
        [StringLength(450)]
        public string Contact { get; set; }

        [StringLength(450)]
        [Required(ErrorMessage = "请输入内容，且内容长度不能超过450")]
        public string Content { get; set; }

        [Required(ErrorMessage = "请选择反馈类型")]
        public FeedbackType? Type { get; set; }
    }
}
