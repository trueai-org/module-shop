using Newtonsoft.Json;

namespace Shop.Module.StorageSm.Models
{
    public class SmResult
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public SmDataResult Data { get; set; }
    }
}
