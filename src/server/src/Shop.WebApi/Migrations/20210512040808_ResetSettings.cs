using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.WebApi.Migrations
{
    public partial class ResetSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "EmailSenderSmtpOptions");

            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "HangfireOptions");

            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "MiniProgramOptions");

            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "SmsSenderAliyunOptions");

            migrationBuilder.UpdateData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "ShopConfig",
                column: "Value",
                value: "{\"ShopName\":\"天网商城\",\"CacheTimeInMinutes\":60,\"RedisCachingEnabled\":false,\"RedisCachingConnection\":\"demo-shop-redis:6379,password=qGmNTf*******,allowAdmin=true\"}");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "ShopConfig",
                column: "Value",
                value: "{\"ShopName\":\"天网商城\",\"CacheTimeInMinutes\":60,\"RedisCachingEnabled\":true,\"RedisCachingConnection\":\"demo-shop-redis:6379,ssl=False,password=qGmNTfPtbQYMX6Qz,allowadmin=true\"}");

            migrationBuilder.InsertData(
                table: "Core_AppSetting",
                columns: new[] { "Id", "FormatType", "IsVisibleInCommonSettingPage", "Module", "Note", "Type", "Value" },
                values: new object[,]
                {
                    { "EmailSenderSmtpOptions", 1, true, "EmailSenderSmtp", null, "Shop.Module.EmailSenderSmtp.EmailSenderSmtpOptions, Shop.Module.EmailSenderSmtp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "{\"SmtpUserName\":\"system@trueai.org\",\"SmtpPassword\":\"******\",\"SmtpHost\":\"smtp.mxhichina.com\",\"SmtpPort\":587}" },
                    { "SmsSenderAliyunOptions", 1, true, "SmsSenderAliyun", null, "Shop.Module.SmsSenderAliyun.Models.SmsSenderAliyunOptions, Shop.Module.SmsSenderAliyun, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "{\"RegionId\":\"default\",\"AccessKeyId\":\"LTAI8v8Crr******\",\"AccessKeySecret\":\"BVZY9tPKjNsuAg2njEgZa1KI******\",\"IsTest\":true}" },
                    { "HangfireOptions", 1, true, "Hangfire", null, "Shop.Module.Hangfire.Models.HangfireOptions, Shop.Module.Hangfire, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "{\"Provider\":3,\"MySqlHangfireConnection\":\"server=172.16.250.6;PORT=3306;database=demo_shop_hangfire;uid=root;pwd=czYj9hnFaVUfZgm7;Connection Timeout=60;Allow Zero Datetime=True;Allow User Variables=True;pooling=true;min pool size=5;max pool size=512;SslMode=None;\",\"SqlServerHangfireConnection\":\"Data Source=127.0.0.1;Initial Catalog=ShopHangfire;User ID=sa;Password=******\",\"RedisHangfireConnection\":\"127.0.0.1:16379,password=******,allowadmin=true\",\"Username\":\"admin\",\"Password\":\"123456\"}" },
                    { "MiniProgramOptions", 1, true, "MiniProgram", null, "Shop.Module.Core.MiniProgram.Models.MiniProgramOptions, Shop.Module.Core.MiniProgram, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "{\"AppId\":\"wxc90c1e6038******\",\"AppSecret\":\"736192139a4ca393de4a18ce22******\",\"MchId\":\"1526******\",\"Key\":\"jtA9J79YNUXnTRpVntHqTGSF2h******\"}" }
                });
        }
    }
}
