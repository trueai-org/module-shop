using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Hangfire.Models;

namespace Shop.Module.Hangfire.Data
{
    public class HangfireCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var option = new HangfireOptions()
            {
                Provider = (ProviderType)(int.Parse(cfg[$"{nameof(HangfireOptions)}:{nameof(HangfireOptions.Provider)}"])),
                MySqlHangfireConnection = cfg[$"{nameof(HangfireOptions)}:{nameof(HangfireOptions.MySqlHangfireConnection)}"],
                SqlServerHangfireConnection = cfg[$"{nameof(HangfireOptions)}:{nameof(HangfireOptions.SqlServerHangfireConnection)}"],
                RedisHangfireConnection = cfg[$"{nameof(HangfireOptions)}:{nameof(HangfireOptions.RedisHangfireConnection)}"],
                Username = cfg[$"{nameof(HangfireOptions)}:{nameof(HangfireOptions.Username)}"],
                Password = cfg[$"{nameof(HangfireOptions)}:{nameof(HangfireOptions.Password)}"]
            };
            var json = JsonConvert.SerializeObject(option);

            modelBuilder.Entity<AppSetting>().HasData(
                 new AppSetting(nameof(HangfireOptions))
                 {
                     Module = "Hangfire",
                     IsVisibleInCommonSettingPage = true,
                     Value = json,
                     FormatType = AppSettingFormatType.Json,
                     Type = typeof(HangfireOptions).AssemblyQualifiedName
                 }
             );
        }
    }
}
