using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Core.Cache;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 管理后台控制器用于处理缓存相关操作的 API 请求。
    /// </summary>
    [ApiController]
    [Route("api/caches")]
    [Authorize(Roles = "admin")]
    public class CacheApiController : ControllerBase
    {
        private readonly IStaticCacheManager _cache;

        public CacheApiController(IStaticCacheManager cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 清除所有缓存。
        /// </summary>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpDelete("clear")]
        public async Task<Result> Upload()
        {
            _cache.Clear();
            await Task.CompletedTask;
            return Result.Ok();
        }
    }
}