namespace Shop.Infrastructure
{
    public class AuthenticationConfig
    {
        public AuthenticationJwtConfig Jwt { get; set; } = new AuthenticationJwtConfig();
    }

    public class AuthenticationJwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }

        /// <summary>
        /// 提示：暂未开启JWT过期策略，此时间用于令牌生成过期时间及令牌续签过期时间
        /// </summary>
        public int AccessTokenDurationInMinutes { get; set; }
    }
}