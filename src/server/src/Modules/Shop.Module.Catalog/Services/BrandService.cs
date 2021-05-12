using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
{
    public class BrandService : IBrandService
    {
        private readonly string _key = CatalogKeys.BrandAll;

        private readonly IRepository<Brand> _brandRepository;
        private readonly IStaticCacheManager _cache;

        public BrandService(IRepository<Brand> brandRepository,
            IStaticCacheManager cache)
        {
            _brandRepository = brandRepository;
            _cache = cache;
        }

        public async Task<IList<Brand>> GetAllByCache()
        {
            var result = await _cache.GetAsync(_key, async () =>
            {
                return await _brandRepository.Query().ToListAsync();
            });
            return result;
        }

        public async Task Create(Brand brand)
        {
            using (var transaction = _brandRepository.BeginTransaction())
            {
                _brandRepository.Add(brand);
                await _brandRepository.SaveChangesAsync();
                transaction.Commit();
            }
            await ClearCache();
        }

        public async Task Update(Brand brand)
        {
            await _brandRepository.SaveChangesAsync();
            await ClearCache();
        }

        public async Task<Result<StandardTableResult<BrandResult>>> List(StandardTableParam param)
        {
            var query = _brandRepository.Query().Where(x => !x.IsDeleted);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                if (search.name != null)
                {
                    string name = search.name;
                    query = query.Where(x => x.Name.Contains(name));
                }
            }

            var gridData = await query.ToStandardTableResult(
                param,
                model => new BrandResult
                {
                    Id = model.Id,
                    IsPublished = model.IsPublished,
                    Name = model.Name,
                    Slug = model.Slug,
                    Description = model.Description,
                    CreatedOn = model.CreatedOn,
                    UpdatedOn = model.UpdatedOn
                });

            return Result.Ok(gridData);
        }

        public async Task ClearCache()
        {
            await Task.Run(() =>
            {
                _cache.Remove(_key);
            });
        }
    }
}
