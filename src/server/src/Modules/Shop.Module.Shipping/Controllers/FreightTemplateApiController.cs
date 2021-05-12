using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Shipping.Entities;
using Shop.Module.Shipping.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Shipping.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/shippings/freight-templates")]
    public class FreightTemplateApiController : ControllerBase
    {
        private readonly IRepository<FreightTemplate> _freightTemplateRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<StateOrProvince> _provinceRepository;
        private readonly IRepository<PriceAndDestination> _priceAndDestinationRepository;
        private readonly IRepository<Product> _productRepository;

        public FreightTemplateApiController(
            IRepository<FreightTemplate> freightTemplateRepository,
            IWorkContext workContext,
            IRepository<StateOrProvince> provinceRepository,
            IRepository<PriceAndDestination> priceAndDestinationRepository,
            IRepository<Product> productRepository)
        {
            _freightTemplateRepository = freightTemplateRepository;
            _workContext = workContext;
            _provinceRepository = provinceRepository;
            _priceAndDestinationRepository = priceAndDestinationRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<Result> Get()
        {
            var query = _freightTemplateRepository.Query();
            var result = await query.Select(x => new FreightTemplateQueryResult
            {
                Id = x.Id,
                Name = x.Name,
                Note = x.Note,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn
            }).ToListAsync();
            return Result.Ok(result);
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<FreightTemplateQueryResult>>> DataList([FromBody]StandardTableParam param)
        {
            var query = _freightTemplateRepository.Query();
            var result = await query
                .ToStandardTableResult(param, x => new FreightTemplateQueryResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Note = x.Note,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                });
            return Result.Ok(result);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody]FreightTemplateCreateParam model)
        {
            _freightTemplateRepository.Add(new FreightTemplate()
            {
                Note = model.Note,
                Name = model.Name
            });
            await _freightTemplateRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody]FreightTemplateCreateParam model)
        {
            var template = await _freightTemplateRepository.FirstOrDefaultAsync(id);
            if (template == null)
                return Result.Fail("运费模板不存在");
            template.Name = model.Name;
            template.Note = model.Note;
            template.UpdatedOn = DateTime.Now;
            await _freightTemplateRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var template = await _freightTemplateRepository.FirstOrDefaultAsync(id);
            if (template == null)
                return Result.Fail("运费模板不存在");

            var any = _priceAndDestinationRepository.Query().Any(c => c.FreightTemplateId == id);
            if (any)
                return Result.Fail("运费模板已被引用，不允许删除");

            var anyProduct = _productRepository.Query().Any(c => c.FreightTemplateId == id);
            if (anyProduct)
                return Result.Fail("运费模板已被产品引用，不允许删除");

            template.IsDeleted = true;
            template.UpdatedOn = DateTime.Now;
            await _freightTemplateRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}