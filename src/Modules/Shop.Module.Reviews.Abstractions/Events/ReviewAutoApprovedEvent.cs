using MediatR;

namespace Shop.Module.Reviews.Abstractions.Events
{
    public class ReviewAutoApprovedEvent : INotification
    {
        public int ReviewId { get; set; }
    }
}
