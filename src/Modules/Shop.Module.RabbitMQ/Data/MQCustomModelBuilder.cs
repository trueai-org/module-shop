using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.RabbitMQ.Models;

namespace Shop.Module.RabbitMQ.Data
{
    public class MQCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var cfg = GlobalConfiguration.Configuration;
            var config = new RabbitMQOptions()
            {
                ConnectionString = cfg[$"{nameof(RabbitMQOptions)}:{nameof(RabbitMQOptions.ConnectionString)}"]
            };
            var json = JsonConvert.SerializeObject(config);
            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting(nameof(RabbitMQOptions))
                {
                    Module = "RabbitMQ",
                    IsVisibleInCommonSettingPage = true,
                    Value = json,
                    FormatType = AppSettingFormatType.Json,
                    Type = typeof(RabbitMQOptions).AssemblyQualifiedName
                }
             );
        }
    }
}
