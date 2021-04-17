using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.StorageGitHub.Models;

namespace Shop.Module.StorageGitHub.Data
{
    public class GitHubCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var option = new StorageGitHubOptions()
            {
                Host = cfg[$"{nameof(StorageGitHubOptions)}:{nameof(StorageGitHubOptions.Host)}"],
                RepositoryName = cfg[$"{nameof(StorageGitHubOptions)}:{nameof(StorageGitHubOptions.RepositoryName)}"],
                BranchName = cfg[$"{nameof(StorageGitHubOptions)}:{nameof(StorageGitHubOptions.BranchName)}"],
                PersonalAccessToken = cfg[$"{nameof(StorageGitHubOptions)}:{nameof(StorageGitHubOptions.PersonalAccessToken)}"],
                SavePath = cfg[$"{nameof(StorageGitHubOptions)}:{nameof(StorageGitHubOptions.SavePath)}"],
            };
            var json = JsonConvert.SerializeObject(option);
            modelBuilder.Entity<AppSetting>().HasData(
                 new AppSetting(nameof(StorageGitHubOptions))
                 {
                     Module = "StorageGitHub",
                     IsVisibleInCommonSettingPage = true,
                     Value = json,
                     FormatType = AppSettingFormatType.Json,
                     Type = typeof(StorageGitHubOptions).AssemblyQualifiedName
                 }
             );
        }
    }
}
