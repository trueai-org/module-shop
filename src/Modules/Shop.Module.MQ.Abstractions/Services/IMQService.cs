using System;
using System.Threading.Tasks;

namespace Shop.Module.MQ.Abstractions.Services
{
    public interface IMQService
    {
        Task DirectSend<T>(string queue, T message) where T : class;

        bool DirectReceive<T>(string queue, Action<T> callback, out string msg) where T : class;
    }
}
