using Newtonsoft.Json;

namespace Shop.Module.StorageSm.Models
{
    public class SmDataResult
    {
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("storename")]
        public string Storename { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("delete")]
        public string Delete { get; set; }
    }
}
