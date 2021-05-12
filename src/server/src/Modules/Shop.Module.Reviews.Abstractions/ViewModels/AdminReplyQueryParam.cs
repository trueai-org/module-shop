using Shop.Module.Reviews.Models;

namespace Shop.Module.Reviews.ViewModels
{
    public class AdminReplyQueryParam
    {
        public ReplyStatus? Status { get; set; }

        public string ReplierName { get; set; }
    }
}
