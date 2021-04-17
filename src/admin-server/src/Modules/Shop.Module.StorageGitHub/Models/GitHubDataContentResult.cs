using Newtonsoft.Json;

namespace Shop.Module.StorageGitHub.Models
{
    public class GitHubDataContentResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("sha")]
        public string Sha { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
        [JsonProperty("git_url")]
        public string GitUrl { get; set; }
        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
