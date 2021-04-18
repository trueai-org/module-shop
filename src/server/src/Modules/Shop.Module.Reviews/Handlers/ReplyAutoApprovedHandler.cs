using MediatR;
using Shop.Infrastructure.Data;
using Shop.Module.Reviews.Abstractions.Events;
using Shop.Module.Reviews.Abstractions.Models;
using Shop.Module.Reviews.Abstractions.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Reviews.Handlers
{
    public class ReplyAutoApprovedHandler : INotificationHandler<ReplyAutoApprovedEvent>
    {
        private readonly IRepository<Reply> _repository;

        public ReplyAutoApprovedHandler(IRepository<Reply> repository)
        {
            _repository = repository;
        }
        public async Task Handle(ReplyAutoApprovedEvent notification, CancellationToken cancellationToken)
        {
            if (notification?.ReplyId > 0)
            {
                var reply = await _repository.FirstOrDefaultAsync(notification.ReplyId);
                if (reply != null && reply.Status == ReplyStatus.Pending)
                {
                    reply.Status = ReplyStatus.Approved;
                    reply.UpdatedOn = DateTime.Now;
                    await _repository.SaveChangesAsync();
                }
            }
        }
    }
}
