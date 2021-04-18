using MassTransit;
using Shop.Module.MQ.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ
{
    public class MemoryMQService : IMQService
    {
        private readonly IBusControl _busControl;

        public MemoryMQService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public async Task Send<T>(string queue, T message) where T : class
        {
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"loopback://localhost/{queue}"));
            await sendEndpoint.Send(message);
        }
    }
}
