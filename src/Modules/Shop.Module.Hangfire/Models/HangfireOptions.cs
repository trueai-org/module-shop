namespace Shop.Module.Hangfire.Models
{
    public class HangfireOptions
    {
        public ProviderType Provider { get; set; } = ProviderType.MySql;

        public string MySqlHangfireConnection { get; set; }

        public string SqlServerHangfireConnection { get; set; }

        public string RedisHangfireConnection { get; set; }

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
