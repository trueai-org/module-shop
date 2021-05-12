namespace Shop.Module.Hangfire.Models
{
    /// <summary>
    /// Hangfire 作业配置
    /// </summary>
    public class HangfireOptions
    {
        /// <summary>
        /// 是否启用 Redis，如果未启用，则默认为内存模式
        /// </summary>
        public bool RedisEnabled { get; set; }

        /// <summary>
        /// Redis 连接字符串
        /// <see cref=""/>
        /// </summary>
        public string RedisConnection { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
