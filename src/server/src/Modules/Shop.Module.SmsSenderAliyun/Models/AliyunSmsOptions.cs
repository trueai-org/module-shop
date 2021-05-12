namespace Shop.Module.SmsSenderAliyun.Models
{
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
