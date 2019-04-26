using MassTransit;
using Shop.Module.MQ.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Shop.Module.MassTransitMQ.Services
{
    public class MQService : IMQService
    {
        private readonly IBusControl _busControl;

        public MQService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public bool DirectReceive<T>(string queue, Action<T> callback, out string msg) where T : class
        {
            throw new NotImplementedException();

            //msg = string.Empty;
            //try
            //{
            //    var handle = _busControl.ConnectConsumer<ProductViewMQConsumer>();
            //    //var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            //    //{
            //    //    cfg.ReceiveEndpoint(queue, e =>
            //    //    {
            //    //        e.Handler<T>(async context =>
            //    //        {
            //    //            await Task.Run(() =>
            //    //            {
            //    //                callback(context.Message);
            //    //            });
            //    //        });
            //    //    });
            //    //});
            //    //busControl.StartAsync();
            //}
            //catch (Exception ex)
            //{
            //    msg = ex.ToString();
            //    return false;
            //}
            //return true;
        }

        public async Task DirectSend<T>(string queue, T message) where T : class
        {
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"loopback://localhost/{queue}"));
            await sendEndpoint.Send(message);
        }
    }
}
