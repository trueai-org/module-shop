using MediatR;

namespace Shop.Module.Reviews.Events
{
    public class ReviewAutoApprovedEvent : INotification
    {
        public int ReviewId { get; set; }
    }
}
