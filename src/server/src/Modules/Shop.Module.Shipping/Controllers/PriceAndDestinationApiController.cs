using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Shipping.Entities;
using Shop.Module.Shipping.ViewModels;

namespace Shop.Module.Shipping.Controllers
{
    /// <summary>
    /// 运费价格与目的地 API 控制器，负责管理运费模板内的具体运费规则。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/shippings/price-destinations")]
    public class PriceAndDestinationApiController : ControllerBase
    {
        private readonly IRepository<FreightTemplate> _freightTemplateRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<StateOrProvince> _provinceRepository;
        private readonly IRepository<PriceAndDestination> _priceAndDestinationRepository;

        public PriceAndDestinationApiController(
            IRepository<FreightTemplate> freightTemplateRepository,
            IWorkContext workContext,
            IRepository<StateOrProvince> provinceRepository,
            IRepository<PriceAndDestination> priceAndDestinationRepository)
        {
            _freightTemplateRepository = freightTemplateRepository;
            _workContext = workContext;
            _provinceRepository = provinceRepository;
            _priceAndDestinationRepository = priceAndDestinationRepository;
        }

        /// <summary>
        /// 根据运费模板 ID 分页获取运费策略列表。
        /// </summary>
        /// <param name="freightTemplateId">运费模板 ID。</param>
        /// <param name="param">分页参数。</param>
        /// <returns>分页的运费策略列表。</returns>
        [HttpPost("grid/{freightTemplateId:int:min(1)}")]
        public async Task<Result<StandardTableResult<PriceAndDestinationQueryResult>>> DataList(int freightTemplateId, [FromBody] StandardTableParam param)
        {
            var query = _priceAndDestinationRepository
                .Query(c => c.FreightTemplateId == freightTemplateId);
            StateOrProvinceLevel? level = null;
            var result = await query
                .Include(c => c.Country)
                .Include(c => c.StateOrProvince)
                .ToStandardTableResult(param, x => new PriceAndDestinationQueryResult
                {
                    Id = x.Id,
                    CountryId = x.CountryId,
                    FreightTemplateId = x.FreightTemplateId,
                    MinOrderSubtotal = x.MinOrderSubtotal,
                    ShippingPrice = x.ShippingPrice,
                    StateOrProvinceId = x.StateOrProvinceId,
                    Note = x.Note,
                    CountryName = x.Country.Name,
                    StateOrProvinceName = x.StateOrProvince != null ? x.StateOrProvince.Name : null,
                    StateOrProvinceLevel = x.StateOrProvince != null ? x.StateOrProvince.Level : level,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    IsEnabled = x.IsEnabled
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 在指定运费模板下创建新的运费策略。
        /// </summary>
        /// <param name="freightTemplateId">运费模板 ID。</param>
        /// <param name="model">运费策略的创建参数。</param>
        /// <returns>创建操作的结果。</returns>
        [HttpPost("{freightTemplateId:int:min(1)}")]
        public async Task<Result> Post(int freightTemplateId, [FromBody] PriceAndDestinationCreateParam model)
        {
            var entity = new PriceAndDestination()
            {
                CountryId = model.CountryId,
                FreightTemplateId = freightTemplateId,
                MinOrderSubtotal = model.MinOrderSubtotal,
                StateOrProvinceId = model.StateOrProvinceId,
                ShippingPrice = model.ShippingPrice,
                Note = model.Note,
                IsEnabled = model.IsEnabled
            };
            var any = await _priceAndDestinationRepository.Query().AnyAsync(c => c.FreightTemplateId == entity.FreightTemplateId
            && c.CountryId == model.CountryId
            && c.StateOrProvinceId == model.StateOrProvinceId);
            if (any)
                return Result.Fail("运费策略已存在，同一国家、省市区运费策略只能存在一个");

            _priceAndDestinationRepository.Add(entity);
            await _priceAndDestinationRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定 ID 的运费策略。
        /// </summary>
        /// <param name="id">运费策略 ID。</param>
        /// <param name="model">运费策略的更新参数。</param>
        /// <returns>更新操作的结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] PriceAndDestinationCreateParam model)
        {
            var entity = await _priceAndDestinationRepository.FirstOrDefaultAsync(id);
            if (entity == null)
                return Result.Fail("单据不存在");

            var any = await _priceAndDestinationRepository.Query().AnyAsync(c => c.FreightTemplateId == entity.FreightTemplateId
            && c.CountryId == model.CountryId
            && c.StateOrProvinceId == model.StateOrProvinceId
            && c.Id != entity.Id);
            if (any)
                return Result.Fail("运费策略已存在，同一国家、省市区运费策略只能存在一个");

            entity.CountryId = model.CountryId;
            entity.MinOrderSubtotal = model.MinOrderSubtotal;
            entity.StateOrProvinceId = model.StateOrProvinceId;
            entity.ShippingPrice = model.ShippingPrice;
            entity.Note = model.Note;
            entity.IsEnabled = model.IsEnabled;
            entity.UpdatedOn = DateTime.Now;
            await _priceAndDestinationRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定 ID 的运费策略。
        /// </summary>
        /// <param name="id">运费策略 ID。</param>
        /// <returns>删除操作的结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var entity = await _priceAndDestinationRepository.FirstOrDefaultAsync(id);
            if (entity == null)
                return Result.Fail("单据不存在");

            entity.IsDeleted = true;
            entity.UpdatedOn = DateTime.Now;
            await _priceAndDestinationRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}