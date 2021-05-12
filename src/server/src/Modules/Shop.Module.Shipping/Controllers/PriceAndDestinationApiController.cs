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
using System;
using System.Threading.Tasks;

namespace Shop.Module.Shipping.Controllers
{
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

        [HttpPost("grid/{freightTemplateId:int:min(1)}")]
        public async Task<Result<StandardTableResult<PriceAndDestinationQueryResult>>> DataList(int freightTemplateId, [FromBody]StandardTableParam param)
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

        [HttpPost("{freightTemplateId:int:min(1)}")]
        public async Task<Result> Post(int freightTemplateId, [FromBody]PriceAndDestinationCreateParam model)
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

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody]PriceAndDestinationCreateParam model)
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