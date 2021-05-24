using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.WebApi.Filters;
using Shop.WebApi.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shop.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomizedConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            GlobalConfiguration.WebRootPath = env.WebRootPath;
            GlobalConfiguration.ContentRootPath = env.ContentRootPath;
            GlobalConfiguration.Configuration = configuration;

            services.AddModules(configuration);
            services.AddCustomizedDataStore(configuration);
            services.AddCustomizedIdentity(configuration);

            services.AddHttpClient();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));

            // why??
            // services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddTransient<IAuthorizationHandler, PermissionHandler>();

            var sp = services.BuildServiceProvider();
            var moduleInitializers = sp.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.ConfigureServices(services, configuration);
            }

            services.AddMvc(options =>
            {
                options.Filters.Add<CustomActionFilterAttribute>();
                options.Filters.Add<CustomExceptionFilterAttribute>();
            })
            .AddNewtonsoftJson(options =>
            {
                // 忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // 不使用驼峰样式的key
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                // 设置输入/输出时间格式
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local; // json to datetime 2019-02-26T22:34:13.000Z -> 2019-02-27 06:34:13
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // 验证错误会自动触发HTTP 400响应。禁止ModelState无效时自动返回错误，仅在api项目中配置。
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-2.2
                // 在用户名中允许的字符。
                // 由于允许用户名、邮箱、手机号登录，因此用户名不能与邮箱命名冲突
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
                // 如果设置为true则邮箱不能为空，因此不设置此值
                //options.User.RequireUniqueEmail = true;

                // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/2fa?view=aspnetcore-1.1&viewFallbackFrom=aspnetcore-2.2
                // 用于防止暴力攻击的帐户锁定
                // Default Lockout settings.
                // 用户已被锁定，如果启用了锁定前的失败的访问尝试数。
                // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // 时间量用户已锁定时在锁定时发生。
                // options.Lockout.MaxFailedAccessAttempts = 5;
                // 确定是否新用户启用锁定功能。默认：true
                // options.Lockout.AllowedForNewUsers = false;

                // 单一登录
                // IsNotAllowed控制，如果全部设置为true，则必须邮箱和手机必须全部验证通过才允许登录。因此暂不开启。
                // Default SignIn settings.
                // 需要已确认的电子邮件，登录。
                //options.SignIn.RequireConfirmedEmail = true;
                //// 需要确认的电话号码进行登录。
                //options.SignIn.RequireConfirmedPhoneNumber = true;
            });

            // mediatR
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            var modules = configuration.GetSection("Modules").Get<List<ModuleInfo>>();

            foreach (var module in modules)
            {
                GlobalConfiguration.Modules.Add(module);

                module.Assembly = Assembly.Load(new AssemblyName(module.Id));

                var moduleType = module.Assembly.GetTypes().FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
                if ((moduleType != null) && (moduleType != typeof(IModuleInitializer)))
                {
                    services.AddSingleton(typeof(IModuleInitializer), moduleType);
                }
            }
        }

        public static void AddCustomizedDataStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<ShopDbContext>(options => options.UseCustomizedDataStore(configuration));
        }

        public static void UseCustomizedDataStore(this DbContextOptionsBuilder options, IConfiguration configuration)
        {
            // SQL Server
            //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Shop.WebApi"));

            // MySql
            options.UseMySql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Shop.WebApi"));
        }

        public static void AddCustomizedIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddUserStore<ShopUserStore>()
            .AddRoleStore<ShopRoleStore>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                // 302
                // or [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false, // 允许匿名
                    ValidateLifetime = false, // in this case, we don't care about the token's expiration date
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration[$"{nameof(AuthenticationOptions)}:Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[$"{nameof(AuthenticationOptions)}:Jwt:Key"]))
                };

                // 某些场景下，我们可能会使用Url来传递Token
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrWhiteSpace(token))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
