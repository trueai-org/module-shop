using Newtonsoft.Json;
using Shop.Module.Payments.Models;

namespace Shop.Module.Core.MiniProgram.Models
{
    public class PaymentOrderResponse : PaymentOrderBaseResponse
    {
        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }

        [JsonProperty("nonceStr")]
        public string NonceStr { get; set; }

        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("signType")]
        public string SignType { get; set; }

        [JsonProperty("paySign")]
        public string PaySign { get; set; }
    }
}
