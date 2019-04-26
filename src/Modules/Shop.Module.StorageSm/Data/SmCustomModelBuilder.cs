using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.StorageSm.Models;

namespace Shop.Module.StorageSm.Data
{
    public class SmCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var option = new StorageSmOptions()
            {
                CustomDomain = cfg[$"{nameof(StorageSmOptions)}:{nameof(StorageSmOptions.CustomDomain)}"]
            };
            var json = JsonConvert.SerializeObject(option);
            modelBuilder.Entity<AppSetting>().HasData(
                 new AppSetting(nameof(StorageSmOptions))
                 {
                     Module = "StorageSm",
                     IsVisibleInCommonSettingPage = true,
                     Value = json,
                     FormatType = AppSettingFormatType.Json,
                     Type = typeof(StorageSmOptions).AssemblyQualifiedName
                 }
             );
        }
    }
}
