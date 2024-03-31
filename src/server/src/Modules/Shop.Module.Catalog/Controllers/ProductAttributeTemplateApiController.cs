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
    /// 产品属性模板的API控制器，负责管理产品属性模板的相关操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("/api/product-attribute-templates")]
    public class ProductAttributeTemplateApiController : ControllerBase
    {
        private readonly IRepository<ProductAttributeTemplate> _productAttrTempRepo;
        private readonly IRepository<ProductAttribute> _productAttrRepo;
        private readonly IRepository<ProductAttributeTemplateRelation> _productAttrTempRelaRepo;

        public ProductAttributeTemplateApiController(
            IRepository<ProductAttributeTemplate> productAttrTemplate,
            IRepository<ProductAttribute> productAttrRepository,
            IRepository<ProductAttributeTemplateRelation> productAttrTempRelaRepo)
        {
            _productAttrTempRepo = productAttrTemplate;
            _productAttrRepo = productAttrRepository;
            _productAttrTempRelaRepo = productAttrTempRelaRepo;
        }

        /// <summary>
        /// 获取所有产品属性模板的列表。
        /// </summary>
        /// <returns>返回产品属性模板的列表。</returns>
        [HttpGet]
        public async Task<Result> Get()
        {
            var result = await _productAttrTempRepo.Query().Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据给定的参数获取产品属性模板的分页列表。
        /// </summary>
        /// <param name="param">分页和筛选参数。</param>
        /// <returns>返回满足条件的产品属性模板的分页结果。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<ProductAttributeTemplateResult>>> DataList([FromBody] StandardTableParam param)
        {
            var query = _productAttrTempRepo.Query()
                .Include(c => c.ProductAttributes)
                .ThenInclude(c => c.Attribute)
                .ThenInclude(c => c.Group);
            var result = await query
                .ToStandardTableResult(param, x => new ProductAttributeTemplateResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Attributes = x.ProductAttributes.Select(c => new ProductAttributeResult
                    {
                        Id = c.AttributeId,
                        Name = c.Attribute.Name,
                        GroupId = c.Attribute.GroupId,
                        GroupName = c.Attribute.Group.Name,
                    }).ToList()
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据指定的ID获取一个产品属性模板。
        /// </summary>
        /// <param name="id">产品属性模板的ID。</param>
        /// <returns>返回指定ID的产品属性模板。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var first = await _productAttrTempRepo.Query()
                .Include(c => c.ProductAttributes)
                .ThenInclude(c => c.Attribute)
                .ThenInclude(c => c.Group)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (first == null)
                return Result.Fail("单据不存在");
            var model = new ProductAttributeTemplateResult
            {
                Id = first.Id,
                Name = first.Name,
                Attributes = first.ProductAttributes.Select(c => new ProductAttributeResult
                {
                    Id = c.AttributeId,
                    Name = c.Attribute.Name,
                    GroupId = c.Attribute.GroupId,
                    GroupName = c.Attribute.Group.Name,
                }).ToList()
            };
            return Result.Ok(model);
        }

        /// <summary>
        /// 添加一个新的产品属性模板。
        /// </summary>
        /// <param name="model">包含产品属性模板信息的参数对象。</param>
        /// <returns>返回添加操作的结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] ProductAttributeTemplateParam model)
        {
            var template = new ProductAttributeTemplate
            {
                Name = model.Name
            };
            var attributeIds = model.AttributeIds.Distinct();
            if (attributeIds.Count() > 0)
            {
                var attrIds = await _productAttrRepo
                    .Query(c => attributeIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync();
                foreach (var attrId in attrIds)
                {
                    template.AddAttribute(attrId);
                }
            }
            _productAttrTempRepo.Add(template);
            await _productAttrTempRepo.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的产品属性模板。
        /// </summary>
        /// <param name="id">需要更新的产品属性模板ID。</param>
        /// <param name="model">包含更新信息的产品属性模板参数对象。</param>
        /// <returns>返回更新操作的结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] ProductAttributeTemplateParam model)
        {
            var productTemplate = await _productAttrTempRepo
                .Query()
                .Include(x => x.ProductAttributes)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (productTemplate == null)
                return Result.Fail("单据不存在");

            productTemplate.Name = model.Name;
            productTemplate.UpdatedOn = DateTime.Now;

            var attributeIds = model.AttributeIds.Distinct();
            var attrIds = new List<int>();
            if (attributeIds.Count() > 0)
            {
                attrIds = await _productAttrRepo
                    .Query(c => attributeIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync();
                foreach (var attrId in attrIds)
                {
                    if (productTemplate.ProductAttributes.Any(x => x.AttributeId == attrId))
                    {
                        continue;
                    }
                    productTemplate.AddAttribute(attrId);
                }
            }

            var deletedAttributes = productTemplate.ProductAttributes.Where(attr => !attrIds.Contains(attr.AttributeId));

            foreach (var deletedAttribute in deletedAttributes)
            {
                deletedAttribute.IsDeleted = true;
                deletedAttribute.UpdatedOn = DateTime.Now;
            }
            _productAttrTempRepo.SaveChanges();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的产品属性模板。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var first = await _productAttrTempRepo.Query().Include(c => c.ProductAttributes).FirstOrDefaultAsync(c => c.Id == id);
            if (first == null)
                return Result.Fail("单据不存在");

            foreach (var item in first.ProductAttributes)
            {
                item.IsDeleted = true;
                item.UpdatedOn = DateTime.Now;
            }

            //var any = await _productAttrTempRelaRepo.Query().AnyAsync(c => c.TemplateId == first.Id);
            //if (any)
            //    return Result.Fail("Please make sure this template not used");

            first.IsDeleted = true;
            first.UpdatedOn = DateTime.Now;
            await _productAttrTempRepo.SaveChangesAsync();
            return Result.Ok();
        }
    }
}