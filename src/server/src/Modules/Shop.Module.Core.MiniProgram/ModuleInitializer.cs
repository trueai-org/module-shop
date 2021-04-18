using Essensoft.AspNetCore.Payment.WeChatPay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.Core.MiniProgram.Services;
using Shop.Module.Payments.Abstractions.Services;

namespace Shop.Module.Core.MiniProgram
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider();
            var json = configuration.GetRequiredService<IConfiguration>().GetValue<string>(nameof(MiniProgramOptions));
            var config = JsonConvert.DeserializeObject<MiniProgramOptions>(json) ?? new MiniProgramOptions();

            services.AddWeChatPay();
            services.Configure<WeChatPayOptions>(options =>
            {
                options.AppId = config.AppId;
                options.MchId = config.MchId;
                options.Secret = config.AppSecret;
                options.Key = config.Key;
            });
            services.AddScoped<IPaymentService, PaymentService>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
