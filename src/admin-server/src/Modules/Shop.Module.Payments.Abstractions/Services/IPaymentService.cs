using Shop.Module.Payments.Abstractions.Models;
using System.Threading.Tasks;

namespace Shop.Module.Payments.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<PaymentOrderBaseResponse> GeneratePaymentOrder(PaymentOrderRequest request);
    }
}
