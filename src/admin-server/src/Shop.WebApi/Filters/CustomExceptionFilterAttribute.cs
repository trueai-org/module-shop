using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shop.Infrastructure;

namespace Shop.WebApi.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                // 内部异常
                context.Result = new JsonResult(Result.Fail(context.Exception.Message));

                // 暂不使用500
                // context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            base.OnException(context);
        }
    }
}
