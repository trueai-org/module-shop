namespace Shop.Module.Core.MiniProgram.Models
{
    public class MiniProgramOptions
    {
        /// <summary>
        /// 微信分配的小程序ID
        /// </summary>
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 商户API密钥
        /// </summary>
        public string Key { get; set; }
    }
}
