using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shop.Module.Core.Abstractions.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.WebApi.Handlers
{
    public class PermissionHandler : IAuthorizationHandler
    {
        private readonly IWorkContext _workContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PermissionHandler(IWorkContext workContext, IHttpContextAccessor httpContextAccessor)
        {
            _workContext = workContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var isAuthenticated = context.User?.Identity?.IsAuthenticated;
            if (isAuthenticated == true)
            {
                // 注意：令牌失效，需返回401。默认返回403
                //if (context.Resource is AuthorizationFilterContext)
                //{
                //    var httpContext = (context.Resource as AuthorizationFilterContext).HttpContext;
                //}

                var httpContext = _httpContextAccessor?.HttpContext;
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
                {
                    token = httpContext.Request.Query["access_token"];
                }

                if (string.IsNullOrWhiteSpace(identityId) || string.IsNullOrWhiteSpace(token) || !int.TryParse(identityId, out int userId) || userId <= 0)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    string path;
                    if (httpContext.GetEndpoint() is RouteEndpoint endpoint && endpoint != null)
                    {
                        path = endpoint.RoutePattern.RawText;
                    }
                    else
                    {
                        path = httpContext.Request?.Path.Value;
                    }

                    path = $"{httpContext.Request.Method}:/{path?.Trim().Trim('/')}";

                    // 访问期间验证令牌并自动续签
                    if (!_workContext.ValidateToken(userId, token.Substring($"{JwtBearerDefaults.AuthenticationScheme} ".Length).Trim(), out int statusCode, path))
                    {
                        if (statusCode != StatusCodes.Status403Forbidden)
                        {
                            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }
    }
}
