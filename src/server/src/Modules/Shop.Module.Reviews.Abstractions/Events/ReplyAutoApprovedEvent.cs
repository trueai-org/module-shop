using MediatR;

namespace Shop.Module.Reviews.Events
{
    public class ReplyAutoApprovedEvent : INotification
    {
        public int ReplyId { get; set; }
    }
}
