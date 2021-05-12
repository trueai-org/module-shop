using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/api/product-attribute-groups")]
    public class ProductAttributeGroupApiController : ControllerBase
    {
        private readonly IRepository<ProductAttributeGroup> _productAttrGroupRepository;
        private readonly IRepository<ProductAttribute> _productAttrRepository;

        public ProductAttributeGroupApiController(IRepository<ProductAttributeGroup> productAttrGroupRepository,
            IRepository<ProductAttribute> productAttrRepository)
        {
            _productAttrGroupRepository = productAttrGroupRepository;
            _productAttrRepository = productAttrRepository;
        }

        [HttpGet]
        public async Task<Result> Get()
        {
            var options = await _productAttrGroupRepository.Query().Select(c => new ProductAttributeGroupParam
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return Result.Ok(options);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
            if (group == null)
                return Result.Fail("单据不存在");
            var model = new ProductAttributeGroupParam
            {
                Id = group.Id,
                Name = group.Name
            };
            return Result.Ok(model);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody]ProductAttributeGroupParam model)
        {
            var group = new ProductAttributeGroup
            {
                Name = model.Name
            };
            _productAttrGroupRepository.Add(group);
            await _productAttrGroupRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody]ProductAttributeGroupParam model)
        {
            var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
            if (group == null)
                return Result.Fail("单据不存在");
            group.Name = model.Name;
            group.UpdatedOn = DateTime.Now;
            await _productAttrGroupRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
            if (group == null)
                return Result.Fail("单据不存在");

            // 验证组是否被属性使用
            var any = await _productAttrRepository.Query().AnyAsync(c => c.GroupId == group.Id);
            if (any)
                return Result.Fail("Please make sure this group not used");

            group.IsDeleted = true;
            group.UpdatedOn = DateTime.Now;
            await _productAttrGroupRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }

}
