using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Module.Orders.Events;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ.Services
{
    public class PaymentReceivedMQConsumer : IConsumer<PaymentReceived>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public PaymentReceivedMQConsumer(
            ILogger<PaymentReceivedMQConsumer> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PaymentReceived> context)
        {
            try
            {
                if (context?.Message != null)
                {
                    await _mediator.Publish(context.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "收到付款消息，处理失败", context?.Message);
            }
        }
    }
}