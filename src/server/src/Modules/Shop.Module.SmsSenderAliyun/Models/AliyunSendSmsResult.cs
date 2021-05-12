namespace Shop.Module.SmsSenderAliyun.Models
{
    public class AliyunSendSmsResult
    {
        /// <summary>
        /// 发送回执ID，可根据该ID在接口QuerySendDetails中查询具体的发送状态。
        /// 900619746936498440^0
        /// </summary>
        public string BizId { get; set; }

        /// <summary>
        /// 请求状态码。
        /// OK
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 状态码的描述。
        /// OK
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求ID。
        /// F655A8D5-B967-440B-8683-DAD6FF8DE990
        /// </summary>
        public string RequestId { get; set; }
    }
}
