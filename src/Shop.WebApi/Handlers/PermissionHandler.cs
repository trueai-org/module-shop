using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Shop.Infrastructure.Helpers;
using Shop.Module.Core.Abstractions.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.WebApi.Handlers
{
    public class PermissionHandler : IAuthorizationHandler
    {
        private readonly ITokenService _tokenService;
        public PermissionHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var isAuthenticated = context.User?.Identity?.IsAuthenticated;
            if (isAuthenticated == true)
            {
                // JWT 验证通过
                // 自定义验证规则、通过缓存处理JWT只能存在一个有效的
                // 注意：令牌失效，需返回401。默认返回403
                if (context.Resource is AuthorizationFilterContext)
                {
                    var httpContext = (context.Resource as AuthorizationFilterContext).HttpContext;
                    if (httpContext == null)
                    {
                        context.Fail();
                        return;
                    }

                    // https://stackoverflow.com/questions/51119926/jwt-authentication-usermanager-getuserasync-returns-null
                    // default the value of UserIdClaimType is ClaimTypes.NameIdentifier
                    var identityId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                    string token = httpContext.Request.Headers["Authorization"];
                    if (string.IsNullOrWhiteSpace(token))
                        token = httpContext.Request.Query["access_token"];

                    // TODO 暂时放开控制，允许一个账号多处登录

                    // 访问期间令牌自动续签（一次性令牌除外）
                    //if (string.IsNullOrWhiteSpace(identityId) ||
                    //    string.IsNullOrWhiteSpace(token) ||
                    //    !_tokenService.ValidateToken(identityId, token.TrimStart(JwtBearerDefaults.AuthenticationScheme).Trim()))
                    //{
                    //    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //}
                }
            }
            await Task.CompletedTask;

            //var isAuthenticated = context.User?.Identity?.IsAuthenticated;
            //if (isAuthenticated == true)
            //{
            //    // JWT 验证通过
            //    // 自定义验证规则、通过缓存处理JWT只能存在一个有效的
            //    // 注意：令牌失效，需返回401。默认返回403
            //    if (context.Resource is AuthorizationFilterContext)
            //    {
            //        var httpContext = (context.Resource as AuthorizationFilterContext).HttpContext;
            //        if (httpContext == null)
            //        {
            //            context.Fail();
            //            return;
            //        }

            //        var identityId = context.User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            //        if (string.IsNullOrWhiteSpace(identityId))
            //        {
            //            //context.Fail();
            //            //BUG,待修改 注意这里需要返回401,且不再继续执行,如果直接返回401并不会阻止方法调用
            //            context.Fail();
            //            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            return;
            //        }

            //        string token = httpContext.Request.Headers["Authorization"];
            //        if (string.IsNullOrWhiteSpace(token))
            //            token = httpContext.Request.Query["access_token"];
            //        if (string.IsNullOrWhiteSpace(token))
            //        {
            //            //context.Fail();
            //            context.Fail();
            //            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            return;
            //        }

            //        // 访问期间令牌自动续签（一次性令牌除外）
            //        if (!_tokenService.ValidateToken(identityId, token.TrimStart(JwtBearerDefaults.AuthenticationScheme).Trim()))
            //        {
            //            //context.Fail();
            //            context.Fail();
            //            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            return;
            //        }

            //        //var bearerAuthResult = await httpContext?.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            //        //var principal = new ClaimsPrincipal();
            //        //if (bearerAuthResult?.Principal != null)
            //        //{
            //        //    principal.AddIdentities(bearerAuthResult.Principal.Identities);
            //        //}

            //        //// 判断请求是否停止
            //        //var handlerProvider = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            //        //var schemeProvider = httpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            //        //foreach (var scheme in await schemeProvider.GetRequestHandlerSchemesAsync())
            //        //{
            //        //    var handler = await handlerProvider.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler;
            //        //    if (handler != null && await handler.HandleRequestAsync())
            //        //    {
            //        //        context.Fail();
            //        //    }
            //        //}

            //        //// 请求Url
            //        //// var questUrl = httpContext.Request.Path.Value.ToLower();
            //        //var defaultAuthenticate = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
            //        //if (defaultAuthenticate != null)
            //        //{
            //        //    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
            //        //    // result?.Principal不为空即登录成功
            //        //    if (result?.Principal != null)
            //        //    {
            //        //        httpContext.User = result.Principal;

            //        //        // 权限中是否存在请求的url
            //        //        if (Requirement.Permissions.GroupBy(g => g.Url).Where(w => w.Key.ToLower() == questUrl).Count() > 0)
            //        //        {
            //        //            var name = httpContext.User.Claims.SingleOrDefault(s => s.Type == requirement.ClaimType).Value;
            //        //            //验证权限
            //        //            if (Requirement.Permissions.Where(w => w.Name == name && w.Url.ToLower() == questUrl).Count() <= 0)
            //        //            {
            //        //                //无权限跳转到拒绝页面
            //        //                httpContext.Response.Redirect(requirement.DeniedAction);
            //        //            }
            //        //        }
            //        //    }
            //        //}

            //        //var pendingRequirements = context.PendingRequirements.ToList();
            //        //foreach (var requirement in pendingRequirements)
            //        //{
            //        //}
            //    }
            //}
        }
    }
}
