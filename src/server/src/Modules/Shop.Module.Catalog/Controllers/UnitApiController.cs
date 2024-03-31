using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 单位API控制器，负责管理商品单位。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("/api/units")]
    public class UnitApiController : ControllerBase
    {
        private readonly string _key = CatalogKeys.UnitAll;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IStaticCacheManager _cache;

        public UnitApiController(
            IRepository<Unit> unitRepository,
            IRepository<Product> productRepository,
            IStaticCacheManager cache)
        {
            _unitRepository = unitRepository;
            _productRepository = productRepository;
            _cache = cache;
        }

        /// <summary>
        /// 获取所有单位信息。
        /// </summary>
        /// <returns>所有单位的列表。</returns>
        [HttpGet]
        public async Task<Result> Get()
        {
            var list = await GetAllByCache();
            var result = list.OrderBy(c => c.Name);
            return Result.Ok(result);
        }

        /// <summary>
        /// 创建一个新的单位。
        /// </summary>
        /// <param name="model">包含单位名称的参数对象。</param>
        /// <returns>操作结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] NameParam model)
        {
            _unitRepository.Add(new Unit { Name = model.Name });
            await _unitRepository.SaveChangesAsync();
            await ClearCache();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的单位名称。
        /// </summary>
        /// <param name="id">要更新的单位ID。</param>
        /// <param name="model">包含新单位名称的参数对象。</param>
        /// <returns>操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] NameParam model)
        {
            var unit = await _unitRepository.FirstOrDefaultAsync(id);
            if (unit == null)
                return Result.Fail("单位不存在");
            unit.Name = model.Name;
            unit.UpdatedOn = DateTime.Now;
            await _unitRepository.SaveChangesAsync();
            await ClearCache();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的单位。
        /// </summary>
        /// <param name="id">要删除的单位ID。</param>
        /// <returns>操作结果，如果单位已被使用，则不允许删除。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var unit = await _unitRepository.FirstOrDefaultAsync(id);
            if (unit == null)
                return Result.Fail("单位不存在");

            var any = await _productRepository.Query().AnyAsync(c => c.UnitId == id);
            if (any)
                return Result.Fail("单位已被使用，不允许删除");

            unit.IsDeleted = true;
            unit.UpdatedOn = DateTime.Now;
            await _unitRepository.SaveChangesAsync();
            await ClearCache();
            return Result.Ok();
        }

        /// <summary>
        /// 清除所有单位信息的缓存。
        /// </summary>
        /// <returns>操作结果。</returns>
        [HttpPost("clear-cache")]
        public async Task<Result> ClearAllCache()
        {
            await ClearCache();
            return Result.Ok();
        }

        private async Task ClearCache()
        {
            await Task.Run(() =>
            {
                _cache.Remove(_key);
            });
        }

        private async Task<IList<Unit>> GetAllByCache()
        {
            return await _cache.GetAsync(_key, async () =>
            {
                return await _unitRepository.Query().ToListAsync();
            });
        }
    }
}