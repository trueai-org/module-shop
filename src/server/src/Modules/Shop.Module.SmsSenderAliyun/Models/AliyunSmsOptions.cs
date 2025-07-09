namespace Shop.Module.SmsSenderAliyun.Models
{
    /// <summary>
    /// 阿里云短信服务配置选项类
    /// 用于配置阿里云短信服务的连接和认证信息
    /// </summary>
    public class AliyunSmsOptions
    {
        /// <summary>
        /// 地域ID
        /// default
        /// cn-hangzhou
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// RAM账号的AccessKey ID
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// RAM账号Access Key Secret
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// 是否为测试短信
        /// </summary>
        public bool IsTest { get; set; }
    }
}
