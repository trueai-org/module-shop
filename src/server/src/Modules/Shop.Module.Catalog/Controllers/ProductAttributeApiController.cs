using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
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

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<ProductAttributeResult>>> DataList([FromBody]StandardTableParam param)
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

        [HttpPost("data/{attributeId:int:min(1)}/grid")]
        public async Task<Result<StandardTableResult<ProductAttributeDataQueryResult>>> DataList(int attributeId, [FromBody]StandardTableParam<ValueParam> param)
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

        [HttpPost("data/{attributeId:int:min(1)}")]
        public async Task<Result> AddData(int attributeId, [FromBody]ProductAttributeDataParam model)
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

        [HttpPut("data/{id:int:min(1)}")]
        public async Task<Result> EditData(int id, [FromBody]ProductAttributeDataParam model)
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
