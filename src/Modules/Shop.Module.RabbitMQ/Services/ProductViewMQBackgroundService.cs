using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shop.Module.Catalog.Abstractions.Events;
using Shop.Module.Core.Abstractions.Events;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.MQ.Abstractions.Data;
using Shop.Module.MQ.Abstractions.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.RabbitMQ.Services
{
    public class ProductViewMQBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IMQService _mqService;
        private readonly IServiceProvider _serviceProvider;

        public ProductViewMQBackgroundService(
            ILogger<ProductViewMQBackgroundService> logger,
            IMQService mqService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _mqService = mqService;
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var success = _mqService.DirectReceive<ProductViewed>(QueueKeys.ProductView, async c =>
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                        throw new Exception("任务已取消");
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Publish(new ProductViewed
                        {
                            EntityId = c.EntityId,
                            UserId = c.UserId,
                            EntityTypeWithId = EntityTypeWithId.Product
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("消息处理失败", ex, c);
                }
            }, out string message);
            if (!success)
            {
                _logger.LogError("消息接收异常", message);
            }
            await Task.CompletedTask;

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            //}
        }
    }
}
