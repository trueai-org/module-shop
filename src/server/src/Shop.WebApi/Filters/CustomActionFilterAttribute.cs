using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shop.Infrastructure;
using System.Linq;

namespace Shop.WebApi.Filters
{
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 由于PermissionHandler暂时无法直接返回401,暂时调整为在方法执行前验证401
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                var result = Result.Fail("登录已失效，请重新登录");
                context.Result = new JsonResult(result);
            }
            else if (context.HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var result = Result.Fail("您无权限访问");
                context.Result = new JsonResult(result);
            }
            else
            {
                if (!context.ModelState.IsValid)
                {
                    var error = context.ModelState.Values.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage ?? "参数异常";
                    context.Result = new JsonResult(Result.Fail(error));
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
