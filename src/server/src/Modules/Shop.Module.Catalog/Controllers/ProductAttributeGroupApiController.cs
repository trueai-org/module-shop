using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 商品属性组API控制器，负责商品属性组的管理操作，如查询、创建、更新和删除。
    /// </summary>
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

        /// <summary>
        /// 获取所有商品属性组的列表。
        /// </summary>
        /// <returns>返回商品属性组的列表。</returns>
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


        /// <summary>
        /// 根据商品属性组ID获取指定商品属性组的详细信息。
        /// </summary>
        /// <param name="id">商品属性组ID。</param>
        /// <returns>返回指定商品属性组的详细信息。</returns>
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

        /// <summary>
        /// 添加新商品属性组。
        /// </summary>
        /// <param name="model">包含商品属性组信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] ProductAttributeGroupParam model)
        {
            var group = new ProductAttributeGroup
            {
                Name = model.Name
            };
            _productAttrGroupRepository.Add(group);
            await _productAttrGroupRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的商品属性组信息。
        /// </summary>
        /// <param name="id">商品属性组ID。</param>
        /// <param name="model">包含商品属性组更新信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] ProductAttributeGroupParam model)
        {
            var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
            if (group == null)
                return Result.Fail("单据不存在");
            group.Name = model.Name;
            group.UpdatedOn = DateTime.Now;
            await _productAttrGroupRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的商品属性组。
        /// </summary>
        /// <param name="id">商品属性组ID。</param>
        /// <returns>返回操作结果。</returns>
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