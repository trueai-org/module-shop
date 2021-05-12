using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using System;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
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

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<BrandResult>>> List([FromBody]StandardTableParam param)
        {
            var result = await _brandService.List(param);
            return result;
        }

        [HttpGet]
        public async Task<Result> Get()
        {
            var brandList = await _brandService.GetAllByCache();
            return Result.Ok(brandList);
        }

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

        [HttpPost("clear-cache")]
        public async Task<Result> ClearCache()
        {
            await _brandService.ClearCache();
            return Result.Ok();
        }
    }
}
