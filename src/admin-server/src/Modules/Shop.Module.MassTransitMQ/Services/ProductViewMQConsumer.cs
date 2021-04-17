using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Module.Catalog.Abstractions.Events;
using Shop.Module.Core.Abstractions.Events;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ.Services
{
    public class ProductViewMQConsumer : IConsumer<ProductViewed>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ProductViewMQConsumer(
            ILogger<ProductViewMQConsumer> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ProductViewed> context)
        {
            try
            {
                if (context?.Message != null)
                {
                    await _mediator.Publish(new EntityViewed
                    {
                        EntityId = context.Message.EntityId,
                        UserId = context.Message.UserId,
                        EntityTypeWithId = context.Message.EntityTypeWithId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "产品浏览记录消息，处理失败", context?.Message);
            }
        }
    }
}

