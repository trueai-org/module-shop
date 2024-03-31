using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 品牌管理API控制器，提供品牌的增删改查等功能。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("/api/brands")]
    public class BrandApiController : ControllerBase
    {
        private readonly IRepository<Brand> _brandRepository;
        private readonly IBrandService _brandService;

        public BrandApiController(IRepository<Brand> brandRepository, IBrandService brandService)
        {
            _brandRepository = brandRepository;
            _brandService = brandService;
        }


        /// <summary>
        /// 获取品牌列表，支持分页、排序等功能。
        /// </summary>
        /// <param name="param">标准表格参数，包含分页、排序等信息。</param>
        /// <returns>返回分页的品牌列表数据。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<BrandResult>>> List([FromBody] StandardTableParam param)
        {
            var result = await _brandService.List(param);
            return result;
        }

        /// <summary>
        /// 获取所有品牌的简要信息列表。
        /// </summary>
        /// <returns>返回所有品牌的简要信息列表。</returns>
        [HttpGet]
        public async Task<Result> Get()
        {
            var brandList = await _brandService.GetAllByCache();
            return Result.Ok(brandList);
        }

        /// <summary>
        /// 根据品牌ID获取品牌详细信息。
        /// </summary>
        /// <param name="id">品牌ID。</param>
        /// <returns>返回指定品牌的详细信息，如果品牌不存在则返回错误信息。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var brand = await _brandRepository.FirstOrDefaultAsync(id);
            if (brand == null)
            {
                return Result.Fail("单据不存在");
            }
            var model = new BrandParam
            {
                Id = brand.Id,
                Name = brand.Name,
                Slug = brand.Slug,
                Description = brand.Description,
                IsPublished = brand.IsPublished
            };
            return Result.Ok(model);
        }

        /// <summary>
        /// 创建新品牌。
        /// </summary>
        /// <param name="model">包含品牌信息的参数模型。</param>
        /// <returns>返回操作结果，表示品牌是否成功创建。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] BrandParam model)
        {
            var brand = new Brand
            {
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                IsPublished = model.IsPublished
            };
            await _brandService.Create(brand);
            return Result.Ok();
        }

        /// <summary>
        /// 更新指定ID的品牌信息。
        /// </summary>
        /// <param name="id">品牌ID。</param>
        /// <param name="model">包含品牌更新信息的参数模型。</param>
        /// <returns>返回操作结果，表示品牌信息是否成功更新。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] BrandParam model)
        {
            var brand = await _brandRepository.FirstOrDefaultAsync(id);
            if (brand == null)
            {
                return Result.Fail("单据不存在");
            }
            brand.Description = model.Description;
            brand.Name = model.Name;
            brand.Slug = model.Slug;
            brand.IsPublished = model.IsPublished;
            await _brandService.Update(brand);
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的品牌。
        /// </summary>
        /// <param name="id">品牌ID。</param>
        /// <returns>返回操作结果，表示品牌是否成功删除。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var brand = await _brandRepository.FirstOrDefaultAsync(id);
            if (brand == null)
            {
                return Result.Fail("单据不存在");
            }
            brand.IsDeleted = true;
            brand.UpdatedOn = DateTime.Now;
            await _brandService.Update(brand);
            return Result.Ok();
        }

        /// <summary>
        /// 清除品牌缓存。
        /// </summary>
        /// <returns>返回操作结果，表示品牌缓存是否成功清除。</returns>
        [HttpPost("clear-cache")]
        public async Task<Result> ClearCache()
        {
            await _brandService.ClearCache();
            return Result.Ok();
        }
    }
}