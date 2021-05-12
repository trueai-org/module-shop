using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
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

        [HttpGet]
        public async Task<Result> Get()
        {
            var list = await GetAllByCache();
            var result = list.OrderBy(c => c.Name);
            return Result.Ok(result);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody]NameParam model)
        {
            _unitRepository.Add(new Unit { Name = model.Name });
            await _unitRepository.SaveChangesAsync();
            await ClearCache();
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody]NameParam model)
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
