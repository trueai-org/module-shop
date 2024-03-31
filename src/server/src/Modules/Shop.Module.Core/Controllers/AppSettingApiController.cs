using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 管理后台控制器用于处理应用程序设置相关操作的 API 请求。
    /// </summary>
    [ApiController]
    [Route("api/appsettings")]
    [Authorize(Roles = "admin")]
    public class AppSettingApiController : ControllerBase
    {
        private readonly IRepositoryWithTypedId<AppSetting, string> _appSettingRepository;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IAppSettingService _appSettingService;
        private readonly IWorkContext _workContext;

        public AppSettingApiController(
            IRepositoryWithTypedId<AppSetting, string> appSettingRepository,
            IConfiguration configuration,
            IAppSettingService appSettingService,
            IWorkContext workContext)
        {
            _appSettingRepository = appSettingRepository;
            _configurationRoot = (IConfigurationRoot)configuration;
            _appSettingService = appSettingService;
            _workContext = workContext;
        }

        /// <summary>
        /// 获取应用程序设置列表。
        /// </summary>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpGet]
        public async Task<Result> Get()
        {
            var settings = await _appSettingRepository.Query().Where(x => x.IsVisibleInCommonSettingPage).ToListAsync();
            return Result.Ok(settings.OrderBy(c => c.Module).ThenBy(c => c.FormatType).ThenBy(c => c.Id));
        }

        /// <summary>
        /// 更新应用程序设置。
        /// </summary>
        /// <param name="model">要更新的应用程序设置。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpPut]
        public async Task<Result> Put([FromBody]AppSetting model)
        {
            // 由于涉及高级权限，影响系统运行，目前暂时控制系统用户才可以修改
            // TODO 待优化
            var user = await _workContext.GetCurrentOrThrowAsync();
            if (user.Id != (int)UserWithId.System)
                return Result.Fail("您不是系统管理员！您没有操作权限");

            var setting = await _appSettingRepository.Query().FirstOrDefaultAsync(x => x.Id == model.Id && x.IsVisibleInCommonSettingPage);
            if (setting != null)
            {
                if (setting.FormatType == AppSettingFormatType.Json)
                {
                    var type = Type.GetType(setting.Type);
                    if (type == null)
                    {
                        return Result.Fail("设置类型异常");
                    }
                    var obj = JsonConvert.DeserializeObject(model.Value, type);
                    if (obj == null)
                    {
                        return Result.Fail("设置参数异常");
                    }
                }

                setting.Value = model.Value;
                var count = await _appSettingRepository.SaveChangesAsync();
                if (count > 0)
                {
                    await _appSettingService.ClearCache(setting.Id);
                    _configurationRoot.Reload();

                    //if (setting.FormatType == AppSettingFormatType.Json)
                    //{
                    //    // 绑定方式
                    //    // Singleton/Option
                    //    // 是否可以重新注入绑定??
                    //    var type = Type.GetType(setting.Type);
                    //    if (type == null)
                    //    {
                    //        return Result.Fail("配置类型异常");
                    //    }
                    //    //var obj = type.Assembly.CreateInstance(type.FullName);
                    //    var obj = Activator.CreateInstance(type);
                    //    //var pis = type.GetType().GetProperties();
                    //    //foreach (var pi in pis)
                    //    //{
                    //    //    obj.GetType().GetField(pi.Name).SetValue(obj, objVal[pi.Name]);
                    //    //}
                    //    return Result.Ok();
                    //}
                }
            }
            return Result.Ok();
        }

        //[HttpPut()]
        //public async Task<Result> Puts([FromBody]IList<AppSetting> model)
        //{
        //    var settings = await _appSettingRepository.Query().Where(x => x.IsVisibleInCommonSettingPage).ToListAsync();
        //    foreach (var item in settings)
        //    {
        //        var vm = model.FirstOrDefault(x => x.Id == item.Id);
        //        if (vm != null)
        //        {
        //            item.Value = vm.Value;
        //        }
        //    }
        //    await _appSettingRepository.SaveChangesAsync();
        //    _configurationRoot.Reload();
        //    return Result.Ok();
        //}
    }
}
