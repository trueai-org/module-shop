using Shop.Infrastructure.Models;

namespace Shop.Infrastructure
{
    /// <summary>
    /// 商城基础配置
    /// </summary>
    public class ShopOptions
    {
        /// <summary>
        /// 商城名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 商城环境 DEV FAT UAT PRO
        /// </summary>
        public ShopEnv ShopEnv { get; set; } = ShopEnv.DEV;

        /// <summary>
        /// 启用 IP 限流
        /// </summary>
        public bool IpRateLimitingEnabled { get; set; } = false;

        /// <summary>
        /// 启用客户端限流
        /// </summary>
        public bool ClientRateLimitingEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for caching (instead of default in-memory caching)
        /// Redis or Memeroy, Redis support (used by web farms, Azure, etc). Find more about it at https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/
        /// </summary>
        public bool RedisCachingEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnection { get; set; }

        /// <summary>
        /// Gets the default cache time in minutes
        /// </summary>
        public int CacheTimeInMinutes { get; set; } = 60;

        public string ApiHost { get; set; }

        public string WebHost { get; set; }
    }
}
