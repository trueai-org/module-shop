using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Core.MiniProgram.Models;

namespace Shop.Module.Core.MiniProgram.Data
{
    public class MiniProgramCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var miniProgramOptions = new MiniProgramOptions()
            {
                AppId = cfg[$"{nameof(MiniProgramOptions)}:{nameof(MiniProgramOptions.AppId)}"],
                AppSecret = cfg[$"{nameof(MiniProgramOptions)}:{nameof(MiniProgramOptions.AppSecret)}"],
                MchId = cfg[$"{nameof(MiniProgramOptions)}:{nameof(MiniProgramOptions.MchId)}"],
                Key = cfg[$"{nameof(MiniProgramOptions)}:{nameof(MiniProgramOptions.Key)}"]
            };

            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting(nameof(MiniProgramOptions))
                {
                    Module = "MiniProgram",
                    IsVisibleInCommonSettingPage = true,
                    Value = JsonConvert.SerializeObject(miniProgramOptions),
                    FormatType = AppSettingFormatType.Json,
                    Type = typeof(MiniProgramOptions).AssemblyQualifiedName
                });
        }
    }
}
