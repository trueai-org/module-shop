using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.SmsSenderAliyun.Models;

namespace Shop.Module.SmsSenderAliyun.Data
{
    public class SmsCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var option = new SmsSenderAliyunOptions()
            {
                IsTest = bool.Parse(cfg[$"{nameof(SmsSenderAliyunOptions)}:{nameof(SmsSenderAliyunOptions.IsTest)}"]),
                RegionId = cfg[$"{nameof(SmsSenderAliyunOptions)}:{nameof(SmsSenderAliyunOptions.RegionId)}"],
                AccessKeyId = cfg[$"{nameof(SmsSenderAliyunOptions)}:{nameof(SmsSenderAliyunOptions.AccessKeyId)}"],
                AccessKeySecret = cfg[$"{nameof(SmsSenderAliyunOptions)}:{nameof(SmsSenderAliyunOptions.AccessKeySecret)}"]
            };
            var json = JsonConvert.SerializeObject(option);
            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting(nameof(SmsSenderAliyunOptions))
                {
                    Module = "SmsSenderAliyun",
                    IsVisibleInCommonSettingPage = true,
                    Value = json,
                    FormatType = AppSettingFormatType.Json,
                    Type = typeof(SmsSenderAliyunOptions).AssemblyQualifiedName
                }
             );
        }
    }
}
