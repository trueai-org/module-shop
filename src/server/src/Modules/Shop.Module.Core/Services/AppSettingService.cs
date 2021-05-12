using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shop.Module.Core.Services;
using System;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public class AppSettingService : IAppSettingService
    {
        private const int cacheTimeForSecond = 60; // 默认缓存时间, 绝对过期时间 * 10
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public AppSettingService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<string> Get(string key)
        {
            return await GetByCahche<string>(key);
        }

        public async Task<T> Get<T>()
        {
            return await GetByCahche<T>(typeof(T).Name);
        }

        public async Task<T> Get<T>(string key)
        {
            return await GetByCahche<T>(key);
        }

        public async Task ClearCache(string key)
        {
            _cache.Remove(key);
            await Task.CompletedTask;
        }

        async Task<T> GetByCahche<T>(string key)
        {
            return await _cache.GetOrCreateAsync(key, async (c) =>
            {
                c.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeForSecond));
                c.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheTimeForSecond * 10));

                var json = _configuration.GetValue<string>(key);
                if (json == null)
                    throw new ArgumentNullException(key);

                var type = typeof(T);
                if (type == typeof(string) || type == typeof(int))
                    return (T)Convert.ChangeType(json, type);

                var obj = JsonConvert.DeserializeObject<T>(json);
                if (obj == null)
                    throw new ArgumentNullException(key);

                return await Task.FromResult(obj);
            });
        }
    }
}
