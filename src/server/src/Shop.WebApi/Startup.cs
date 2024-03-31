using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Shop.WebApi.Extensions;
using System.Reflection;

namespace Shop.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomizedConfigureServices(Configuration, Environment);

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Api", Version = "1.0.0" });

            //});

            services.AddSwaggerDoc("Module Shop Api");

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.Use(async (context, next) =>
                {
                    // 处理 Swagger 刷新记住授权状态
                    var isAuthorization = context.Request.Headers.ContainsKey("Authorization");
                    if (!isAuthorization)
                    {
                        var isToken = context.Request.Cookies.ContainsKey("access-token");
                        if (isToken)
                        {
                            var token = context.Request.Cookies["access-token"];
                            context.Request.Headers.TryAdd("Authorization", $"Bearer {token}");
                        }
                    }
                    await next();
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop Api");
            });

            app.UseCustomizedConfigure(env);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/", () =>
                {
                    return "ok";
                });

                endpoints.MapControllers();
            });
        }
    }
}
