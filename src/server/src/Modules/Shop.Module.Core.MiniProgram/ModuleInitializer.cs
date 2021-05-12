using Essensoft.AspNetCore.Payment.WeChatPay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.Core.MiniProgram.Services;
using Shop.Module.Payments.Services;

namespace Shop.Module.Core.MiniProgram
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var opt = new MiniProgramOptions();
            var sec = configuration.GetSection(nameof(MiniProgramOptions));
            sec.Bind(opt);
            services.Configure<MiniProgramOptions>(sec);

            services.AddWeChatPay();
            services.Configure<WeChatPayOptions>(options =>
            {
                options.AppId = opt.AppId;
                options.MchId = opt.MchId;
                options.AppSecret = opt.AppSecret;
                options.Key = opt.Key;
            });
            services.AddScoped<IPaymentService, PaymentService>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
