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
    /// 产品选项API控制器，负责管理产品选项的相关操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("/api/product-options")]
    public class ProductOptionApiController : ControllerBase
    {
        private readonly IRepository<ProductOption> _productOptionRepository;
        private readonly IRepository<ProductOptionData> _productOptionDataRepository;

        public ProductOptionApiController(
            IRepository<ProductOption> productOptionRepository,
            IRepository<ProductOptionData> productOptionDataRepository)
        {
            _productOptionRepository = productOptionRepository;
            _productOptionDataRepository = productOptionDataRepository;
        }

        /// <summary>
        /// 获取所有未被删除的产品选项。
        /// </summary>
        /// <returns>产品选项列表。</returns>
        [HttpGet]
        public async Task<Result> Get()
        {
            var options = await _productOptionRepository.Query().Where(x => !x.IsDeleted).ToListAsync();
            return Result.Ok(options);
        }

        /// <summary>
        /// 根据分页参数获取产品选项的分页列表。
        /// </summary>
        /// <param name="param">分页和筛选参数。</param>
        /// <returns>分页的产品选项列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<ProductOptionResult>>> DataList([FromBody] StandardTableParam param)
        {
            var query = _productOptionRepository.Query();
            var result = await query
                .ToStandardTableResult(param, x => new ProductOptionResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayType = x.DisplayType
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据产品选项ID获取单个产品选项的详细信息。
        /// </summary>
        /// <param name="id">产品选项ID。</param>
        /// <returns>指定的产品选项详情。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var productOption = await _productOptionRepository.FirstOrDefaultAsync(id);
            if (productOption == null)
                return Result.Fail("单据不存在");
            var model = new ProductOptionResult
            {
                Id = productOption.Id,
                Name = productOption.Name,
                DisplayType = productOption.DisplayType
            };
            return Result.Ok(model);
        }

        /// <summary>
        /// 添加一个新的产品选项。
        /// </summary>
        /// <param name="model">产品选项数据。</param>
        /// <returns>操作结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] ProductOptionParam model)
        {
            var productOption = new ProductOption
            {
                Name = model.Name,
                DisplayType = model.DisplayType
            };
            _productOptionRepository.Add(productOption);
            await _productOptionRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的产品选项。
        /// </summary>
        /// <param name="id">产品选项ID。</param>
        /// <param name="model">更新的产品选项数据。</param>
        /// <returns>操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] ProductOptionParam model)
        {
            var productOption = await _productOptionRepository.FirstOrDefaultAsync(id);
            if (productOption == null)
                return Result.Fail("单据不存在");
            productOption.Name = model.Name;
            productOption.DisplayType = model.DisplayType;
            productOption.UpdatedOn = DateTime.Now;
            await _productOptionRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的产品选项。
        /// </summary>
        /// <param name="id">产品选项ID。</param>
        /// <returns>操作结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var productOption = await _productOptionRepository.FirstOrDefaultAsync(id);
            if (productOption == null)
                return Result.Fail("单据不存在");

            var any = _productOptionDataRepository.Query().Any(c => c.OptionId == id);
            if (any)
            {
                return Result.Fail("请确保选项未被值数据引用");
            }

            productOption.IsDeleted = true;
            productOption.UpdatedOn = DateTime.Now;
            await _productOptionRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 获取指定产品选项ID的所有选项数据。
        /// </summary>
        /// <param name="optionId">产品选项ID。</param>
        /// <returns>产品选项数据列表。</returns>
        [HttpGet("data/{optionId:int:min(1)}")]
        public async Task<Result<List<ProductOptionDataListResult>>> DataList(int optionId)
        {
            var query = _productOptionDataRepository.Query(c => c.OptionId == optionId);
            var list = await query.Include(c => c.Option).ToListAsync();
            var result = list.Select(c => new ProductOptionDataListResult
            {
                Id = c.Id,
                Value = c.Value,
                Description = c.Description,
                OptionId = c.OptionId,
                OptionName = c.Option.Name,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                IsPublished = c.IsPublished,
                Display = c.Display,
                OptionDisplayType = c.Option.DisplayType
            }).ToList();
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据分页参数和产品选项ID获取产品选项数据的分页列表。
        /// </summary>
        /// <param name="optionId">产品选项ID。</param>
        /// <param name="param">分页和筛选参数。</param>
        /// <returns>分页的产品选项数据列表。</returns>
        [HttpPost("data/{optionId:int:min(1)}/grid")]
        public async Task<Result<StandardTableResult<ProductOptionDataListResult>>> DataList(int optionId, [FromBody] StandardTableParam<ValueParam> param)
        {
            var query = _productOptionDataRepository.Query().Include(c => c.Option).Where(c => c.OptionId == optionId);
            if (param.Search != null)
            {
                var value = param.Search.Value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    query = query.Where(x => x.Value.Contains(value.Trim()));
                }
            }
            var result = await query.Include(c => c.Option)
                .ToStandardTableResult(param, c => new ProductOptionDataListResult
                {
                    Id = c.Id,
                    Value = c.Value,
                    Description = c.Description,
                    OptionId = c.OptionId,
                    OptionName = c.Option.Name,
                    CreatedOn = c.CreatedOn,
                    UpdatedOn = c.UpdatedOn,
                    IsPublished = c.IsPublished,
                    Display = c.Display,
                    OptionDisplayType = c.Option.DisplayType
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 向指定的产品选项添加新的选项数据。
        /// </summary>
        /// <param name="optionId">产品选项ID。</param>
        /// <param name="model">选项数据。</param>
        /// <returns>操作结果。</returns>
        [HttpPost("data/{optionId:int:min(1)}")]
        public async Task<Result> AddData(int optionId, [FromBody] ProductOptionDataParam model)
        {
            var data = new ProductOptionData
            {
                OptionId = optionId,
                IsPublished = model.IsPublished,
                Value = model.Value,
                Description = model.Description,
                Display = model.Display
            };
            _productOptionDataRepository.Add(data);
            await _productOptionDataRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的产品选项数据。
        /// </summary>
        /// <param name="id">产品选项数据ID。</param>
        /// <param name="model">更新的产品选项数据。</param>
        /// <returns>操作结果。</returns>
        [HttpPut("data/{id:int:min(1)}")]
        public async Task<Result> EditData(int id, [FromBody] ProductOptionDataParam model)
        {
            var data = await _productOptionDataRepository.FirstOrDefaultAsync(id);
            if (data == null)
            {
                return Result.Fail("单据不存在");
            }
            data.IsPublished = model.IsPublished;
            data.Value = model.Value;
            data.Description = model.Description;
            data.Display = model.Display;
            data.UpdatedOn = DateTime.Now;
            await _productOptionDataRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的产品选项数据。
        /// </summary>
        /// <param name="id">产品选项数据ID。</param>
        /// <returns>操作结果。</returns>
        [HttpDelete("data/{id:int:min(1)}")]
        public async Task<Result> DeleteData(int id)
        {
            var data = await _productOptionDataRepository.FirstOrDefaultAsync(id);
            if (data == null)
            {
                return Result.Fail("单据不存在");
            }
            data.IsDeleted = true;
            data.UpdatedOn = DateTime.Now;
            await _productOptionDataRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}