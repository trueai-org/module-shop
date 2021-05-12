using System.Threading.Tasks;

namespace Shop.Module.MQ
{
    public interface IMQService
    {
        Task Send<T>(string queue, T message) where T : class;
    }
}
