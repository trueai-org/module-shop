using MassTransit;
using Shop.Module.MQ;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ
{
    public class RabbitMQService : IMQService
    {
        private readonly IBusControl _busControl;
        public RabbitMQService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public async Task Send<T>(string queue, T message) where T : class
        {
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{queue}"));
            await sendEndpoint.Send(message);
        }
    }
}
