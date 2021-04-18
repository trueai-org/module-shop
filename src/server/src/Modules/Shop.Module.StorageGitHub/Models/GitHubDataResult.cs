using Newtonsoft.Json;

namespace Shop.Module.StorageGitHub.Models
{
    public class GitHubDataResult
    {
        [JsonProperty("content")]
        public GitHubDataContentResult Content { get; set; }
    }
}
