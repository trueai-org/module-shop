using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface IAppSettingService
    {
        Task<string> Get(string key);

        Task<T> Get<T>();

        Task<T> Get<T>(string key);

        Task ClearCache(string key);
    }
}
