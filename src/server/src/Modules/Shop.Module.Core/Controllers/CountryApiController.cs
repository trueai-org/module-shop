using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 管理后台控制器用于处理国家和省份相关操作的 API 请求。
    /// </summary>
    [ApiController]
    [Route("api/countries")]
    [Authorize(Roles = "admin")]
    public class CountryApiController : ControllerBase
    {
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateOrProvince> _provinceRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly ICountryService _countryService;

        public CountryApiController(
            IRepository<Country> countryRepository,
            IRepository<StateOrProvince> provinceRepository,
            IRepository<Address> addressRepository,
            ICountryService countryService)
        {
            _countryRepository = countryRepository;
            _provinceRepository = provinceRepository;
            _addressRepository = addressRepository;
            _countryService = countryService;
        }

        /// <summary>
        /// 获取所有国家的分页结果。
        /// </summary>
        /// <param name="param">分页参数。</param>
        /// <returns>表示操作结果的 <see cref="Result{T}"/> 对象，其中的值为 <see cref="StandardTableResult{CountryResult}"/> 对象。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<CountryResult>>> List([FromBody] StandardTableParam param)
        {
            var query = _countryRepository.Query();
            var result = await query.Include(x => x.StatesOrProvinces)
                  .ToStandardTableResult(param, c => new CountryResult
                  {
                      Id = c.Id,
                      CreatedOn = c.CreatedOn,
                      IsCityEnabled = c.IsCityEnabled,
                      DisplayOrder = c.DisplayOrder,
                      IsBillingEnabled = c.IsBillingEnabled,
                      IsDeleted = c.IsDeleted,
                      IsDistrictEnabled = c.IsDistrictEnabled,
                      IsPublished = c.IsPublished,
                      IsShippingEnabled = c.IsShippingEnabled,
                      Name = c.Name,
                      NumericIsoCode = c.NumericIsoCode,
                      ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                      TwoLetterIsoCode = c.TwoLetterIsoCode,
                      UpdatedOn = c.UpdatedOn,
                      StateOrProvinceCount = c.StatesOrProvinces.Count
                  });
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据国家ID获取国家详情。
        /// </summary>
        /// <param name="id">国家ID。</param>
        /// <returns>指定ID的国家详情。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var country = await _countryRepository.Query()
                .Include(c => c.StatesOrProvinces)
                .Where(c => c.Id == id)
                .Select(c => new CountryResult
                {
                    Id = c.Id,
                    CreatedOn = c.CreatedOn,
                    IsCityEnabled = c.IsCityEnabled,
                    DisplayOrder = c.DisplayOrder,
                    IsBillingEnabled = c.IsBillingEnabled,
                    IsDeleted = c.IsDeleted,
                    IsDistrictEnabled = c.IsDistrictEnabled,
                    IsPublished = c.IsPublished,
                    IsShippingEnabled = c.IsShippingEnabled,
                    Name = c.Name,
                    NumericIsoCode = c.NumericIsoCode,
                    ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                    TwoLetterIsoCode = c.TwoLetterIsoCode,
                    UpdatedOn = c.UpdatedOn,
                    StateOrProvinceCount = c.StatesOrProvinces.Count
                }).FirstOrDefaultAsync();
            if (country == null)
                throw new Exception("国家不存在");
            return Result.Ok(country);
        }

        /// <summary>
        /// 获取所有国家的列表。
        /// </summary>
        /// <returns>所有国家的列表。</returns>
        [HttpGet()]
        public async Task<Result> Get()
        {
            var countries = await _countryRepository.Query()
                .Include(c => c.StatesOrProvinces)
                .Select(c => new CountryResult
                {
                    Id = c.Id,
                    CreatedOn = c.CreatedOn,
                    IsCityEnabled = c.IsCityEnabled,
                    DisplayOrder = c.DisplayOrder,
                    IsBillingEnabled = c.IsBillingEnabled,
                    IsDeleted = c.IsDeleted,
                    IsDistrictEnabled = c.IsDistrictEnabled,
                    IsPublished = c.IsPublished,
                    IsShippingEnabled = c.IsShippingEnabled,
                    Name = c.Name,
                    NumericIsoCode = c.NumericIsoCode,
                    ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                    TwoLetterIsoCode = c.TwoLetterIsoCode,
                    UpdatedOn = c.UpdatedOn,
                    StateOrProvinceCount = c.StatesOrProvinces.Count
                }).ToListAsync();
            return Result.Ok(countries);
        }

        /// <summary>
        /// 添加新的国家。
        /// </summary>
        /// <param name="model">包含新国家信息的对象。</param>
        /// <returns>操作结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] CountryCreateParam model)
        {
            var any = _countryRepository.Query().Any(c => c.NumericIsoCode == model.NumericIsoCode
            || c.TwoLetterIsoCode == model.TwoLetterIsoCode
            || c.ThreeLetterIsoCode == model.ThreeLetterIsoCode);
            if (any)
            {
                return Result.Fail("国家编码已存在");
            }
            var country = new Country()
            {
                DisplayOrder = model.DisplayOrder,
                NumericIsoCode = model.NumericIsoCode,
                ThreeLetterIsoCode = model.ThreeLetterIsoCode,
                TwoLetterIsoCode = model.TwoLetterIsoCode,
                Name = model.Name,
                IsBillingEnabled = model.IsBillingEnabled,
                IsCityEnabled = model.IsCityEnabled,
                IsDistrictEnabled = model.IsDistrictEnabled,
                IsPublished = model.IsPublished,
                IsShippingEnabled = model.IsShippingEnabled
            };
            _countryRepository.Add(country);
            await _countryRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的国家信息。
        /// </summary>
        /// <param name="id">要更新的国家ID。</param>
        /// <param name="model">包含更新信息的对象。</param>
        /// <returns>操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] CountryCreateParam model)
        {
            var country = await _countryRepository.FirstOrDefaultAsync(id);
            if (country == null)
                throw new Exception("国家不存在");
            var any = _countryRepository.Query().Any(c =>
            (c.NumericIsoCode == model.NumericIsoCode
            || c.TwoLetterIsoCode == model.TwoLetterIsoCode
            || c.ThreeLetterIsoCode == model.ThreeLetterIsoCode)
            && c.Id != id);
            if (any)
            {
                return Result.Fail("国家编码已存在");
            }
            country.DisplayOrder = model.DisplayOrder;
            country.NumericIsoCode = model.NumericIsoCode;
            country.ThreeLetterIsoCode = model.ThreeLetterIsoCode;
            country.TwoLetterIsoCode = model.TwoLetterIsoCode;
            country.Name = model.Name;
            country.IsBillingEnabled = model.IsBillingEnabled;
            country.IsCityEnabled = model.IsCityEnabled;
            country.IsDistrictEnabled = model.IsDistrictEnabled;
            country.IsPublished = model.IsPublished;
            country.IsShippingEnabled = model.IsShippingEnabled;
            country.UpdatedOn = DateTime.Now;
            await _countryRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的国家。
        /// </summary>
        /// <param name="id">要删除的国家ID。</param>
        /// <returns>操作结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var country = await _countryRepository.FirstOrDefaultAsync(id);
            if (country == null)
                throw new Exception("国家不存在");

            var any = _provinceRepository.Query().Any(c => c.CountryId == country.Id);
            if (any)
                throw new Exception("请确保国家未被使用");

            var anyUsed = _addressRepository.Query().Any(c => c.CountryId == id);
            if (anyUsed)
                throw new Exception("请确保国家未被使用");

            country.IsDeleted = true;
            country.UpdatedOn = DateTime.Now;
            await _countryRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 获取指定国家ID的省份列表，支持分页。
        /// </summary>
        /// <param name="countryId">国家ID。</param>
        /// <param name="param">分页和查询参数。</param>
        /// <returns>包含省份列表的分页结果。</returns>
        [HttpPost("provinces/grid/{countryId:int:min(1)}")]
        public async Task<Result<StandardTableResult<ProvinceQueryResult>>> ListProvince(int countryId, [FromBody] StandardTableParam<ProvinceQueryParam> param)
        {
            var query = _provinceRepository.Query().Where(c => c.CountryId == countryId);
            var search = param.Search;
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                    query = query.Where(c => c.Name.Contains(search.Name.Trim()));
                if (!string.IsNullOrWhiteSpace(search.Code))
                    query = query.Where(c => c.Code.Contains(search.Code.Trim()));
                if (search.ParentId.HasValue)
                    query = query.Where(c => c.ParentId == search.ParentId);
                if (search.Level.Count > 0)
                    query = query.Where(c => search.Level.Contains(c.Level));
            }
            var result = await query.Include(x => x.Parent)
                  .ToStandardTableResult(param, province => new ProvinceQueryResult
                  {
                      Code = province.Code,
                      CountryId = province.CountryId,
                      CreatedOn = province.CreatedOn,
                      DisplayOrder = province.DisplayOrder,
                      Id = province.Id,
                      IsPublished = province.IsPublished,
                      Level = province.Level,
                      Name = province.Name,
                      ParentId = province.ParentId,
                      ParentName = province.Parent == null ? "" : province.Parent.Name,
                      UpdatedOn = province.UpdatedOn
                  });
            return Result.Ok(result);
        }

        /// <summary>
        /// 获取指定国家ID的省份树结构。
        /// </summary>
        /// <param name="countryId">国家ID。</param>
        /// <returns>省份的树结构列表。</returns>
        [HttpGet("provinces/tree/{countryId:int:min(1)}")]
        public async Task<Result<IList<ProvinceTreeResult>>> ProvinceTree(int countryId)
        {
            var list = await _countryService.GetProvinceByCache(countryId);
            var result = _countryService.ProvinceTree(list);
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据省份ID获取省份详情。
        /// </summary>
        /// <param name="id">省份ID。</param>
        /// <returns>指定ID的省份详情。</returns>
        [HttpGet("provinces/{id:int:min(1)}")]
        public async Task<Result<ProvinceGetResult>> GetProvince(int id)
        {
            var province = await _provinceRepository.Query().Include(c => c.Parent).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (province == null)
                throw new Exception("单据不存在");
            var result = new ProvinceGetResult()
            {
                Code = province.Code,
                CountryId = province.CountryId,
                CreatedOn = province.CreatedOn,
                DisplayOrder = province.DisplayOrder,
                Id = province.Id,
                IsPublished = province.IsPublished,
                Level = province.Level,
                Name = province.Name,
                ParentId = province.ParentId,
                ParentName = province.Parent?.Name,
                UpdatedOn = province.UpdatedOn
            };
            return Result.Ok(result);
        }

        /// <summary>
        /// 在指定国家下添加新的省份。
        /// </summary>
        /// <param name="countryId">国家ID。</param>
        /// <param name="model">包含省份信息的对象。</param>
        /// <returns>操作结果。</returns>
        [HttpPost("provinces/{countryId:int:min(1)}")]
        public async Task<Result> AddProvince(int countryId, [FromBody] ProvinceCreateParam model)
        {
            var anyCountry = _countryRepository.Query().Any(c => c.Id == countryId);
            if (!anyCountry)
                throw new Exception("国家不存在");
            var level = await GetLevelByParent(model.ParentId);
            var any = _provinceRepository.Query().Any(c => c.Name == model.Name && c.CountryId == countryId && c.ParentId == model.ParentId);
            if (any)
                throw new Exception("单据已存在");
            var province = new StateOrProvince()
            {
                CountryId = countryId,
                ParentId = model.ParentId,
                DisplayOrder = model.DisplayOrder,
                Name = model.Name,
                IsPublished = model.IsPublished,
                Level = level,
                Code = model.Code
            };
            _provinceRepository.Add(province);
            await _provinceRepository.SaveChangesAsync();
            await _countryService.ClearProvinceCache(countryId);
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的省份信息。
        /// </summary>
        /// <param name="id">要更新的省份ID。</param>
        /// <param name="model">包含更新信息的对象。</param>
        /// <returns>操作结果。</returns>
        [HttpPut("provinces/{id:int:min(1)}")]
        public async Task<Result> EditProvince(int id, [FromBody] ProvinceCreateParam model)
        {
            var province = await _provinceRepository.FirstOrDefaultAsync(id);
            if (province == null)
                throw new Exception("单据不存在");
            var anyCountry = _countryRepository.Query().Any(c => c.Id == province.CountryId);
            if (!anyCountry)
                throw new Exception("国家不存在");

            var any = _provinceRepository.Query().Any(c => c.Name == model.Name && c.CountryId == province.CountryId && c.ParentId == model.ParentId && c.Id != id);
            if (any)
                throw new Exception("单据已存在");

            if (model.ParentId == province.Id)
                throw new Exception("不能设置自身作为父级");

            //高层级或低层级 升高或降低控制
            //如果当前单据存在子集，则不允许调整层级
            var level = await GetLevelByParent(model.ParentId);
            if (level != province.Level)
            {
                var anyChild = _provinceRepository.Query().Any(c => c.ParentId == province.Id);
                if (anyChild)
                    throw new Exception("当前单据存在子级，不允许调整层级");
            }

            //province.CountryId = model.CountryId;
            province.ParentId = model.ParentId;
            province.DisplayOrder = model.DisplayOrder;
            province.Name = model.Name;
            province.IsPublished = model.IsPublished;
            province.Level = level;
            province.Code = model.Code;
            province.UpdatedOn = DateTime.Now;
            await _provinceRepository.SaveChangesAsync();
            await _countryService.ClearProvinceCache(province.CountryId);
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的省份。
        /// </summary>
        /// <param name="id">要删除的省份ID。</param>
        /// <returns>操作结果。</returns>
        [HttpDelete("provinces/{id:int:min(1)}")]
        public async Task<Result> DeleteProvince(int id)
        {
            var province = await _provinceRepository.FirstOrDefaultAsync(id);
            if (province == null)
                throw new Exception("单据不存在");

            var any = _provinceRepository.Query().Any(c => c.ParentId == id);
            if (any)
                throw new Exception("请确保单据未被使用");

            var anyUsed = _addressRepository.Query().Any(c => c.StateOrProvinceId == id);
            if (anyUsed)
                throw new Exception("请确保单据未被使用");

            province.IsDeleted = true;
            province.UpdatedOn = DateTime.Now;
            await _provinceRepository.SaveChangesAsync();
            await _countryService.ClearProvinceCache(province.CountryId);
            return Result.Ok();
        }

        private async Task<StateOrProvinceLevel> GetLevelByParent(int? parentId)
        {
            var level = StateOrProvinceLevel.Default;
            if (parentId.HasValue)
            {
                var anyParent = _provinceRepository.Query().Any(c => c.Id == parentId.Value);
                if (!anyParent)
                    throw new Exception("父级不存在");

                var parent = await _provinceRepository.FirstOrDefaultAsync(parentId.Value);
                if (parent == null)
                    throw new Exception("父级不存在");

                //如果存在父级，则处理level
                switch (parent.Level)
                {
                    case StateOrProvinceLevel.Default:
                        level = StateOrProvinceLevel.City;
                        break;

                    case StateOrProvinceLevel.City:
                        level = StateOrProvinceLevel.District;
                        break;

                    case StateOrProvinceLevel.District:
                        level = StateOrProvinceLevel.Street;
                        break;

                    case StateOrProvinceLevel.Street:
                        throw new Exception($"无法设置最小层级[{parent.Level.ToString()}]作为父级");
                    default:
                        throw new Exception("父级类型异常");
                }
            }
            return level;
        }
    }
}