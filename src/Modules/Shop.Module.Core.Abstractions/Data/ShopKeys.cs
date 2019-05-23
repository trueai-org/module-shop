using Shop.Infrastructure;

namespace Shop.Module.Core.Abstractions.Data
{
    public class ShopKeys
    {
        // redis key prefix must start with [system]
        public static readonly string System = "shop" + ":" + GlobalConfiguration.Version;

        public static string RegisterPhonePrefix = System + ":register:phone:";

        public static string UserJwtTokenPrefix = System + ":user:jwt:token:";

        public static string Provinces = System + ":country:";

        /// <summary>
        /// 接口地址
        /// </summary>
        public const string ApiHost = "ApiHost";

        /// <summary>
        /// 站点地址
        /// </summary>
        public const string WebHost = "WebHost";

        /// <summary>
        /// 访客用户默认密码
        /// </summary>
        public const string GuestDefaultPassword = "123456";

        /// <summary>
        /// 用户Cookie名称
        /// </summary>
        public const string UserGuidCookiesName = "ShopUserGuid";
    }
}
