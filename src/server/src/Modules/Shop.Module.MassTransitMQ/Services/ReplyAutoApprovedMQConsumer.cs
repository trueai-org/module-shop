using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Module.Reviews.Events;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ.Services
{
    public class ReplyAutoApprovedMQConsumer : IConsumer<ReplyAutoApprovedEvent>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        public ReplyAutoApprovedMQConsumer(
            ILogger<ReplyAutoApprovedMQConsumer> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ReplyAutoApprovedEvent> context)
        {
            try
            {
                if (context?.Message?.ReplyId > 0)
                {
                    await _mediator.Publish(context.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "回复自动审核消息，处理失败", context?.Message);
            }
        }
    }
}

