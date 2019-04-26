using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shop.Infrastructure;

namespace Shop.WebApi.Filters
{
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 由于PermissionHandler暂时无法直接返回401,暂时调整为在方法执行前验证401
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Result = new JsonResult(Result.Fail("请重新登录!"));
            }
            else if (context.HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Result = new JsonResult(Result.Fail("您无权限访问!"));
            }
            else
            {
                if (!context.ModelState.IsValid)
                {
                    // 验证错误
                    context.Result = new JsonResult(Result.Fail(context.ModelState));
                }
            }
            base.OnActionExecuting(context);
        }

        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    //var xx = _memoryCache.GetOrCreate<string>(context.GetHashCode().ToString(), c =>
        //    //{
        //    //    return Guid.NewGuid().ToString();
        //    //});

        //    //既然  context.HttpContext.GetHashCode() 执行前中后，都是一样的，那么其实也可以使用其他缓存
        //    //甚至直接使用 他自身的缓存

        //    //var logService = (IMongoLogService)context.HttpContext.RequestServices.GetService(typeof(IMongoLogService));
        //    //var request = context.HttpContext.Request;
        //    //var model = new MongoApiLog
        //    //{
        //    //    Host = request.Host.Host,
        //    //    Https = request.IsHttps,
        //    //    Path = request.Path,
        //    //    Port = request.Host.Port ?? 0,
        //    //    Url = request.Host.Value + request.Path + (request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) ? request.QueryString.ToString() : ""),
        //    //    Mehod = request.Method,
        //    //    Parameter = request.QueryString.ToString(),
        //    //    SendTime = DateTime.Now,
        //    //    Remark = "begin:" + context.HttpContext.GetHashCode().ToString()
        //    //};
        //    //logService.DirectSend(MongoLogQueue.ApiLogQueue, model);

        //    //  if (!ModelState.IsValid) return BadRequest("参数错误!");
        //    //if (context.ModelState.IsValid) return;

        //    //var stopwach = new Stopwatch();
        //    //stopwach.Start();
        //    //context.HttpContext.Items.Add(Resources.StopwachKey, stopwach);

        //    //var modelState = context.ModelState.FirstOrDefault(f => f.Value.Errors.Any());
        //    //string errorMsg = modelState.Value.Errors.First().ErrorMessage;
        //    //throw new AppException(errorMsg);
        //}

        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    //var logService = (IMongoLogService)context.HttpContext.RequestServices.GetService(typeof(IMongoLogService));
        //    //var request = context.HttpContext.Request;
        //    //var model = new MongoApiLog
        //    //{
        //    //    Host = request.Host.Host,
        //    //    Https = request.IsHttps,
        //    //    Path = request.Path,
        //    //    Port = request.Host.Port ?? 0,
        //    //    Url = request.Host.Value + request.Path + (request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) ? request.QueryString.ToString() : ""),
        //    //    Mehod = request.Method,
        //    //    Parameter = request.QueryString.ToString(),
        //    //    SendTime = DateTime.Now,
        //    //    Remark = "over:" + context.HttpContext.GetHashCode().ToString()
        //    //};
        //    //logService.DirectSend(MongoLogQueue.ApiLogQueue, model);

        //    //var httpContext = context.HttpContext;
        //    //var stopwach = httpContext.Items[Resources.StopwachKey] as Stopwatch;
        //    //stopwach.Stop();
        //    //var time = stopwach.Elapsed;

        //    //if (time.TotalSeconds > 5)
        //    //{
        //    //    var factory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
        //    //    var logger = factory.CreateLogger<ActionExecutedContext>();
        //    //    logger.LogWarning($"{context.ActionDescriptor.DisplayName}执行耗时:{time.ToString()}");
        //    //}
        //    //上面的代码利用使用HttpContext传递一个Stopwach来计算action的执行时间，并在超过5秒时输出警告日志。

        //    //           注册方法与ExceptionFinter相同。找到系统根目录Startup.cs文件，修改ConfigureServices方法如下

        //    //services.AddMvc(options =>
        //    //{
        //    //    options.Filters.Add<ActionFilter>();
        //    //});
        //}
    }
}
