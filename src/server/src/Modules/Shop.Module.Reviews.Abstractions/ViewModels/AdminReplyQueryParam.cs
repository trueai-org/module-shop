using Shop.Module.Reviews.Abstractions.Models;

namespace Shop.Module.Reviews.Abstractions.ViewModels
{
    public class AdminReplyQueryParam
    {
        public ReplyStatus? Status { get; set; }

        public string ReplierName { get; set; }
    }
}
