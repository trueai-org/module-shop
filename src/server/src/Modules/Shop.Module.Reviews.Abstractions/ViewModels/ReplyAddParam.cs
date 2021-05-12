using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReplyAddParam
    {
        public int ReviewId { get; set; }

        /// <summary>
        /// 回复回复
        /// </summary>
        public int? ToReplyId { get; set; }

        [Required]
        [StringLength(400, MinimumLength = 2)]
        public string Comment { get; set; }

        public bool IsAnonymous { get; set; }
    }
}
