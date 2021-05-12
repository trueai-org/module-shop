using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Module.Reviews.Events;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ.Services
{
    public class ReviewAutoApprovedMQConsumer : IConsumer<ReviewAutoApprovedEvent>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        public ReviewAutoApprovedMQConsumer(
            ILogger<ReviewAutoApprovedMQConsumer> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ReviewAutoApprovedEvent> context)
        {
            try
            {
                if (context?.Message?.ReviewId > 0)
                {
                    await _mediator.Publish(context.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "评论自动审核消息，处理失败", context?.Message);
            }
        }
    }
}

