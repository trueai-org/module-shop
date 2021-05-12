using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public class CountryService : ICountryService
    {
        private readonly string _provincesKey = ShopKeys.Provinces;
        private readonly IStaticCacheManager _cache;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateOrProvince> _provinceRepository;
        private readonly IRepository<Address> _addressRepository;

        public CountryService(
            IStaticCacheManager cache,
            IRepository<Country> countryRepository,
            IRepository<StateOrProvince> provinceRepository,
            IRepository<Address> addressRepository)
        {
            _cache = cache;
            _countryRepository = countryRepository;
            _provinceRepository = provinceRepository;
            _addressRepository = addressRepository;
        }

        public async Task<IList<StateOrProvinceDto>> GetProvinceByCache(int countryId)
        {
            var result = await _cache.GetAsync(_provincesKey + countryId, async () =>
            {
                return await _provinceRepository.Query(c => c.CountryId == countryId)
                .OrderBy(c => c.Level).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new StateOrProvinceDto()
                {
                    Id = c.Id,
                    Level = c.Level,
                    Name = c.Name,
                    ParentId = c.ParentId
                }).ToListAsync();
            });
            return result;
        }

        public async Task ClearProvinceCache(int countryId)
        {
            await Task.Run(() =>
            {
                _cache.Remove(_provincesKey + countryId);
            });
        }

        public IList<ProvinceTreeResult> ProvinceTree(IList<StateOrProvinceDto> list, int? parentId = null)
        {
            var result = new List<ProvinceTreeResult>();
            var childrens = list.Where(c => c.ParentId == parentId)
                .Select(c => new ProvinceTreeResult()
                {
                    Key = c.Id.ToString(),
                    Value = c.Id.ToString(),
                    Title = c.Name,
                    Label = c.Name,
                    Children = ProvinceTree(list, c.Id)
                });
            result.AddRange(childrens);
            return result;
        }

        /// <summary>
        /// 省市区 例：8 转换为 ['1', '6', '8']
        /// </summary>
        /// <param name="provinces"></param>
        /// <param name="stateOrProvinceId"></param>
        /// <param name="list"></param>
        /// <param name="loop"></param>
        public void ProvincesTransformToStringArray(IList<StateOrProvinceDto> provinces, int stateOrProvinceId, ref IList<string> list, int loop = 0)
        {
            if (list == null)
                list = new List<string>();
            if (loop >= 5)
                return;

            var model = provinces.FirstOrDefault(c => c.Id == stateOrProvinceId);
            if (model == null)
                return;
            list.Insert(0, model.Id.ToString());
            if (model.ParentId.HasValue && model.ParentId.Value > 0)
            {
                ProvincesTransformToStringArray(provinces, model.ParentId.Value, ref list, loop++);
            }
        }
    }
}
