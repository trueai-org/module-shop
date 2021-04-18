namespace Shop.Infrastructure
{
    public class ShopConfig
    {
        public string ShopName { get; set; }
        /// <summary>
        /// Gets the default cache time in minutes
        /// </summary>
        public int CacheTimeInMinutes { get; set; } = 60;
        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for caching (instead of default in-memory caching)
        /// Redis or Memeroy, Redis support (used by web farms, Azure, etc). Find more about it at https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/
        /// </summary>
        public bool RedisCachingEnabled { get; set; }
        /// <summary>
        /// Gets or sets Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnection { get; set; }
    }
}
