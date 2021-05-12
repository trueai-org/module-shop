using MediatR;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Services;
using Shop.Module.Orders.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Orders.Events
{
    public class PaymentReceivedHandler : INotificationHandler<PaymentReceived>
    {
        private readonly IOrderService _orderService;

        public PaymentReceivedHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Handle(PaymentReceived notification, CancellationToken cancellationToken)
        {
            if (notification == null)
                return;

            await _orderService.PaymentReceived(new PaymentReceivedParam()
            {
                Note = notification.Note,
                OrderId = notification.OrderId,
                OrderNo = notification.OrderNo,
                PaymentFeeAmount = notification.PaymentFeeAmount,
                PaymentMethod = notification.PaymentMethod,
                PaymentOn = notification.PaymentOn
            });
        }
    }
}
