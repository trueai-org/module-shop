using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Shop.Infrastructure;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using System.Runtime.InteropServices;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 管理后台系统服务相关 API
    /// </summary>
    [ApiController]
    [Route("api/system")]
    [Authorize()]
    public class SystemApiController : ControllerBase
    {
        private readonly IAppSettingService _appSettingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemApiController(
            IAppSettingService appSettingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _appSettingService = appSettingService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取系统运行信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public async Task<Result> Get()
        {
            var version = await _appSettingService.Get("Global.AssetVersion");
            var result = new SystemInfoResult()
            {
                Version = version,
                ServerTimeZone = TimeZoneInfo.Local.StandardName,
                ServerLocalTime = DateTime.Now,
                UtcTime = DateTime.UtcNow,
                HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host],
            };

            //ensure no exception is thrown
            try
            {
                result.OperatingSystem = Environment.OSVersion.VersionString;
                result.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
                result.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch { }

            //foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
            //{
            //    result.Headers.Add(new SystemInfoResult.HeaderModel
            //    {
            //        Name = header.Key,
            //        Value = header.Value
            //    });
            //}

            //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    var loadedAssemblyModel = new SystemInfoResult.LoadedAssembly
            //    {
            //        FullName = assembly.FullName
            //    };

            //    //ensure no exception is thrown
            //    try
            //    {
            //        loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
            //        loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
            //            .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

            //        //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
            //        //we use a simple method because the more Jeff Atwood's solution doesn't work anymore
            //        //more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/
            //        loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(System.IO.File.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

            //    }
            //    catch { }
            //    result.LoadedAssemblies.Add(loadedAssemblyModel);
            //}

            return Result.Ok(result);
        }
    }
}