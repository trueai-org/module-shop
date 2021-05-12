using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.WebApi.Migrations
{
    public partial class ResetSettings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "ApiHost");

            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "ShopConfig");

            migrationBuilder.DeleteData(
                table: "Core_AppSetting",
                keyColumn: "Id",
                keyValue: "WebHost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Core_AppSetting",
                columns: new[] { "Id", "FormatType", "IsVisibleInCommonSettingPage", "Module", "Note", "Type", "Value" },
                values: new object[] { "ApiHost", 0, true, "Core", null, null, "https://demo.shopapi.circle.ac.cn/" });

            migrationBuilder.InsertData(
                table: "Core_AppSetting",
                columns: new[] { "Id", "FormatType", "IsVisibleInCommonSettingPage", "Module", "Note", "Type", "Value" },
                values: new object[] { "WebHost", 0, true, "Core", null, null, "https://demo.shop.circle.ac.cn/" });

            migrationBuilder.InsertData(
                table: "Core_AppSetting",
                columns: new[] { "Id", "FormatType", "IsVisibleInCommonSettingPage", "Module", "Note", "Type", "Value" },
                values: new object[] { "ShopConfig", 1, true, "Core", null, "Shop.Infrastructure.ShopConfig, Shop.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "{\"ShopName\":\"天网商城\",\"CacheTimeInMinutes\":60,\"RedisCachingEnabled\":false,\"RedisCachingConnection\":\"demo-shop-redis:6379,password=qGmNTf*******,allowAdmin=true\"}" });
        }
    }
}
