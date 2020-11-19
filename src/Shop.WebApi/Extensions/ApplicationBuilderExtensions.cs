using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;

namespace Shop.WebApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomizedConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            ////跨域
            //app.UseCors(option =>
            //{
            //    option.AllowAnyHeader();
            //    option.AllowAnyMethod();
            //    option.AllowAnyOrigin();
            //    option.AllowCredentials();
            //});

            ////JWT
            //app.UseAuthentication();

            //module
            var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.Configure(app, env);
            }

            //静态资源
            app.UseStaticFiles();
        }
    }
}
