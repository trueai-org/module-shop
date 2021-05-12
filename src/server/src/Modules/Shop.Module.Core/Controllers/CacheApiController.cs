using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Core.Cache;
using System.Threading.Tasks;

namespace Shop.Module.Core.Controllers
{
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

        [HttpDelete("clear")]
        public async Task<Result> Upload()
        {
            _cache.Clear();
            await Task.CompletedTask;
            return Result.Ok();
        }
    }
}

