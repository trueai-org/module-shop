using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 商品属性API控制器，负责商品属性的管理操作，如查询、创建、更新和删除。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/product-attributes")]
    public class ProductAttributeApiController : ControllerBase
    {
        private readonly IRepository<ProductAttribute> _productAttrRepository;
        private readonly IRepository<ProductAttributeData> _productAttrDataRepository;
        private readonly IRepository<ProductAttributeValue> _productAttrValueRepository;
        private readonly IRepository<ProductAttributeTemplateRelation> _productAttrTempRelaRepo;

        public ProductAttributeApiController(
            IRepository<ProductAttribute> productAttrRepository,
            IRepository<ProductAttributeData> productAttrDataRepository,
            IRepository<ProductAttributeValue> productAttrValueRepository,
            IRepository<ProductAttributeTemplateRelation> productAttrTempRelaRepo)
        {
            _productAttrRepository = productAttrRepository;
            _productAttrDataRepository = productAttrDataRepository;
            _productAttrValueRepository = productAttrValueRepository;
            _productAttrTempRelaRepo = productAttrTempRelaRepo;
        }

        /// <summary>
        /// 获取所有商品属性的列表。
        /// </summary>
        /// <returns>返回商品属性的列表。</returns>
        [HttpGet]
        public async Task<Result<List<ProductAttributeResult>>> List()
        {
            var attributes = await _productAttrRepository
                .Query()
                .Where(c => !c.IsDeleted)
                .Select(x => new ProductAttributeResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.Group.Name,
                    GroupId = x.GroupId
                }).ToListAsync();
            return Result.Ok(attributes);
        }

        /// <summary>
        /// 按属性组分组，获取商品属性数组。
        /// </summary>
        /// <returns>返回分组后的商品属性列表。</returns>
        [HttpGet("group-array")]
        public async Task<Result<List<ProductAttributeGroupArrayResult>>> GroupArray()
        {
            var attributes = await _productAttrRepository
                .Query()
                .Where(c => !c.IsDeleted)
                .Select(x => new ProductAttributeResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.Group.Name,
                    GroupId = x.GroupId
                }).ToListAsync();
            var result = attributes.GroupBy(c => c.GroupId).Select(c => new ProductAttributeGroupArrayResult
            {
                GroupId = c.Key,
                GroupName = attributes.FirstOrDefault(x => x.GroupId == c.Key)?.GroupName,
                ProductAttributes = attributes.Where(x => x.GroupId == c.Key).OrderBy(x => x.Name).ToList()
            }).OrderBy(c => c.GroupName).ToList();
            return Result.Ok(result);
        }

        /// <summary>
        /// 分页获取商品属性列表，支持排序等高级功能。
        /// </summary>
        /// <param name="param">包含分页和排序参数的对象。</param>
        /// <returns>返回分页的商品属性列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<ProductAttributeResult>>> DataList([FromBody] StandardTableParam param)
        {
            var query = _productAttrRepository.Query();
            var result = await query.Include(c => c.Group)
                .ToStandardTableResult(param, x => new ProductAttributeResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.Group.Name,
                    GroupId = x.GroupId
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据商品属性ID获取指定商品属性的详细信息。
        /// </summary>
        /// <param name="id">商品属性ID。</param>
        /// <returns>返回指定商品属性的详细信息。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var productAttribute = await _productAttrRepository.Query()
                .Include(c => c.Group)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (productAttribute == null)
            {
                return Result.Fail("单据不存在");
            }
            var model = new ProductAttributeResult
            {
                Id = productAttribute.Id,
                Name = productAttribute.Name,
                GroupId = productAttribute.GroupId,
                GroupName = productAttribute.Group?.Name
            };
            return Result.Ok(model);
        }

        /// <summary>
        /// 添加新商品属性。
        /// </summary>
        /// <param name="model">包含商品属性信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] ProductAttributeParam model)
        {
            var productAttribute = new ProductAttribute
            {
                Name = model.Name,
                GroupId = model.GroupId
            };
            _productAttrRepository.Add(productAttribute);
            await _productAttrRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的商品属性信息。
        /// </summary>
        /// <param name="id">商品属性ID。</param>
        /// <param name="model">包含商品属性更新信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] ProductAttributeParam model)
        {
            var productAttribute = await _productAttrRepository.FirstOrDefaultAsync(id);
            if (productAttribute == null)
            {
                return Result.Fail("单据不存在");
            }
            productAttribute.Name = model.Name;
            productAttribute.GroupId = model.GroupId;
            productAttribute.UpdatedOn = DateTime.Now;
            await _productAttrRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的商品属性。
        /// </summary>
        /// <param name="id">商品属性ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var productAttribute = await _productAttrRepository.FirstOrDefaultAsync(id);
            if (productAttribute == null)
            {
                return Result.Fail("单据不存在");
            }

            var any = _productAttrDataRepository.Query().Any(c => c.AttributeId == id);
            if (any)
            {
                return Result.Fail("请确保属性未被值数据引用");
            }

            any = _productAttrValueRepository.Query().Any(c => c.AttributeId == id);
            if (any)
            {
                return Result.Fail("请确保属性未被产品引用");
            }

            any = _productAttrTempRelaRepo.Query().Any(c => c.AttributeId == id);
            if (any)
            {
                return Result.Fail("请确保属性未被产品模板引用");
            }

            productAttribute.IsDeleted = true;
            productAttribute.UpdatedOn = DateTime.Now;
            await _productAttrRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 根据商品属性ID获取该属性的所有值。
        /// </summary>
        /// <param name="attributeId">商品属性ID。</param>
        /// <returns>返回商品属性值的列表。</returns>
        [HttpGet("data/{attributeId:int:min(1)}")]
        public async Task<Result<List<ProductAttributeDataQueryResult>>> DataList(int attributeId)
        {
            var query = _productAttrDataRepository.Query(c => c.AttributeId == attributeId);
            var list = await query.Include(c => c.Attribute).ToListAsync();
            var result = list.Select(c => new ProductAttributeDataQueryResult
            {
                Id = c.Id,
                Value = c.Value,
                Description = c.Description,
                AttributeId = c.AttributeId,
                AttributeName = c.Attribute.Name,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                IsPublished = c.IsPublished
            }).ToList();
            return Result.Ok(result);
        }

        /// <summary>
        /// 分页获取商品属性值列表，支持排序等高级功能。
        /// </summary>
        /// <param name="attributeId">商品属性ID。</param>
        /// <param name="param">包含分页和排序参数的对象。</param>
        /// <returns>返回分页的商品属性值列表。</returns>
        [HttpPost("data/{attributeId:int:min(1)}/grid")]
        public async Task<Result<StandardTableResult<ProductAttributeDataQueryResult>>> DataList(int attributeId, [FromBody] StandardTableParam<ValueParam> param)
        {
            var query = _productAttrDataRepository.Query(c => c.AttributeId == attributeId);
            if (param.Search != null)
            {
                var value = param.Search.Value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    query = query.Where(x => x.Value.Contains(value.Trim()));
                }
            }
            var result = await query.Include(c => c.Attribute)
                .ToStandardTableResult(param, c => new ProductAttributeDataQueryResult
                {
                    Id = c.Id,
                    Value = c.Value,
                    Description = c.Description,
                    AttributeId = c.AttributeId,
                    AttributeName = c.Attribute.Name,
                    CreatedOn = c.CreatedOn,
                    UpdatedOn = c.UpdatedOn,
                    IsPublished = c.IsPublished
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 为指定的商品属性添加新的属性值。
        /// </summary>
        /// <param name="attributeId">商品属性ID。</param>
        /// <param name="model">包含商品属性值信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPost("data/{attributeId:int:min(1)}")]
        public async Task<Result> AddData(int attributeId, [FromBody] ProductAttributeDataParam model)
        {
            var data = new ProductAttributeData
            {
                AttributeId = attributeId,
                IsPublished = model.IsPublished,
                Value = model.Value,
                Description = model.Description
            };
            _productAttrDataRepository.Add(data);
            await _productAttrDataRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的商品属性值信息。
        /// </summary>
        /// <param name="id">商品属性值ID。</param>
        /// <param name="model">包含商品属性值更新信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("data/{id:int:min(1)}")]
        public async Task<Result> EditData(int id, [FromBody] ProductAttributeDataParam model)
        {
            var data = await _productAttrDataRepository.FirstOrDefaultAsync(id);
            if (data == null)
            {
                return Result.Fail("单据不存在");
            }
            data.IsPublished = model.IsPublished;
            data.Value = model.Value;
            data.Description = model.Description;
            data.UpdatedOn = DateTime.Now;
            await _productAttrDataRepository.SaveChangesAsync();
            return Result.Ok();
        }


        /// <summary>
        /// 删除指定ID的商品属性值。
        /// </summary>
        /// <param name="id">商品属性值ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpDelete("data/{id:int:min(1)}")]
        public async Task<Result> DeleteData(int id)
        {
            var data = await _productAttrDataRepository.FirstOrDefaultAsync(id);
            if (data == null)
            {
                return Result.Fail("单据不存在");
            }
            data.IsDeleted = true;
            data.UpdatedOn = DateTime.Now;
            await _productAttrDataRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}