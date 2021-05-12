using MediatR;
using Shop.Infrastructure.Data;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Events;
using Shop.Module.Reviews.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Reviews.Handlers
{
    public class ReviewAutoApprovedHandler : INotificationHandler<ReviewAutoApprovedEvent>
    {
        private readonly IRepository<Review> _repository;

        public ReviewAutoApprovedHandler(IRepository<Review> repository)
        {
            _repository = repository;
        }
        public async Task Handle(ReviewAutoApprovedEvent notification, CancellationToken cancellationToken)
        {
            if (notification?.ReviewId > 0)
            {
                var review = await _repository.FirstOrDefaultAsync(notification.ReviewId);
                if (review != null && review.Status == ReviewStatus.Pending)
                {
                    review.Status = ReviewStatus.Approved;
                    review.UpdatedOn = DateTime.Now;
                    await _repository.SaveChangesAsync();
                }
            }
        }
    }
}
