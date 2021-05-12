namespace Shop.Module.StorageGitHub.Models
{
    public class StorageGitHubOptions
    {
        /// <summary>
        /// GitHub api host
        /// </summary>
        public string Host { get; set; } = "https://api.github.com/";

        /// <summary>
        /// Repository name
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Branch name
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Personal access token
        /// </summary>
        public string PersonalAccessToken { get; set; }

        /// <summary>
        /// Save path
        /// </summary>
        public string SavePath { get; set; } = "/";
    }
}
