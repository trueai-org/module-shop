using Shop.Module.Orders.Abstractions.Entities;
using Shop.Module.Orders.Abstractions.ViewModels;
using Shop.Module.Payments.Abstractions.Models;
using System.Threading.Tasks;

namespace Shop.Module.Orders.Abstractions.Services
{
    public interface IOrderService
    {
        Task<Order> OrderCreate(int userId, OrderCreateBaseParam param, string adminNote = null);

        Task<OrderCreateResult> OrderCreateByCart(int cartId, OrderCreateBaseParam param, string adminNote = null);

        Task Cancel(int id, int userId, string reason);

        Task<OrderAddressResult> GetOrderAddress(int orderAddressId);

        /// <summary>
        /// 获取订单预支付信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<PaymentOrderBaseResponse> PayInfo(int orderId);

        Task PaymentReceived(PaymentReceivedParam param);

        /// <summary>
        /// 验证并获取预下单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CheckoutResult> OrderCheckout(CheckoutParam param);
    }
}
