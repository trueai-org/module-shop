using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;

namespace Shop.Module.EmailSenderSmtp.Data
{
    public class EmailSenderSmtpCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var config = new EmailSenderSmtpOptions()
            {
                SmtpHost = cfg[$"{nameof(EmailSenderSmtpOptions)}:{nameof(EmailSenderSmtpOptions.SmtpHost)}"],
                SmtpPort = int.Parse(cfg[$"{nameof(EmailSenderSmtpOptions)}:{nameof(EmailSenderSmtpOptions.SmtpPort)}"]),
                SmtpUserName = cfg[$"{nameof(EmailSenderSmtpOptions)}:{nameof(EmailSenderSmtpOptions.SmtpUserName)}"],
                SmtpPassword = cfg[$"{nameof(EmailSenderSmtpOptions)}:{nameof(EmailSenderSmtpOptions.SmtpPassword)}"],
            };
            var json = JsonConvert.SerializeObject(config);
            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting(nameof(EmailSenderSmtpOptions))
                {
                    Module = "EmailSenderSmtp",
                    IsVisibleInCommonSettingPage = true,
                    Value = json,
                    FormatType = AppSettingFormatType.Json,
                    Type = typeof(EmailSenderSmtpOptions).AssemblyQualifiedName
                }
             );
        }
    }
}
