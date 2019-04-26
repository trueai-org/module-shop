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

            //var logService = (IMongoLogService)context.HttpContext.RequestServices.GetService(typeof(IMongoLogService));
            //var request = context.HttpContext.Request;
            //var model = new MongoExceptionLog
            //{
            //    Host = request.Host.Host,
            //    Parameter = request.QueryString.ToString(),
            //    SendTime = DateTime.Now,
            //    Exception = JsonConvert.SerializeObject(new
            //    {
            //        context.Exception.Message,
            //        context.Exception.Source,
            //        context.Exception.StackTrace
            //    }),
            //    Remark = "error:" + context.HttpContext.GetHashCode().ToString()
            //};
            //logService.DirectSend(MongoLogQueue.ExceptionLogQueue, model);

            //var json = new JsonErrorResponse();
            ////这里面是自定义的操作记录日志
            //if (context.Exception.GetType() == typeof(UserOperationException))
            //{
            //    json.Message = context.Exception.Message;
            //    if (_env.IsDevelopment())
            //    {
            //        json.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息
            //    }
            //    context.Result = new BadRequestObjectResult(json);//返回异常数据
            //}
            //else
            //{
            //    json.Message = "发生了未知内部错误";
            //    if (_env.IsDevelopment())
            //    {
            //        json.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息
            //    }
            //    context.Result = new InternalServerErrorObjectResult(json);
            //}
        }
    }
}
