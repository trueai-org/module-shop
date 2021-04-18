using System.Threading.Tasks;

namespace Shop.Module.MQ.Abstractions.Services
{
    public interface IMQService
    {
        Task Send<T>(string queue, T message) where T : class;
    }
}
