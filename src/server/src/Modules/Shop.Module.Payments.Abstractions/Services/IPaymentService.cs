using Shop.Module.Payments.Models;
using System.Threading.Tasks;

namespace Shop.Module.Payments.Services
{
    public interface IPaymentService
    {
        Task<PaymentOrderBaseResponse> GeneratePaymentOrder(PaymentOrderRequest request);
    }
}
