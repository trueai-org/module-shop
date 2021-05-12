using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/categories")]
    public class CategoryApiController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly ICategoryService _categoryService;
        private readonly IMediaService _mediaService;

        public CategoryApiController(
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Media> mediaRepository,
            ICategoryService categoryService,
            IMediaService mediaService)
        {
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _mediaRepository = mediaRepository;
            _categoryService = categoryService;
            _mediaService = mediaService;
        }

        [HttpGet]
        public async Task<Result<IList<CategoryResult>>> Get()
        {
            var result = await _categoryService.GetAll();
            return Result.Ok(result);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var category = await _categoryRepository.Query().Include(x => x.Media).FirstOrDefaultAsync(c => c.Id == id);
            var model = new CategoryParam
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                MetaTitle = category.MetaTitle,
                MetaKeywords = category.MetaKeywords,
                MetaDescription = category.MetaDescription,
                DisplayOrder = category.DisplayOrder,
                Description = category.Description,
                ParentId = category.ParentId,
                IncludeInMenu = category.IncludeInMenu,
                IsPublished = category.IsPublished,
                MediaId = category.MediaId,
                ThumbnailImageUrl = await _mediaService.GetThumbnailUrl(category.Media),
            };
            return Result.Ok(model);
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<CategoryResult>>> List([FromBody]StandardTableParam param)
        {
            var result = await _categoryService.List(param);
            return result;
        }

        [HttpPost("clear-cache")]
        public async Task<Result> ClearCache()
        {
            await _categoryService.ClearCache();
            return Result.Ok();
        }

        [HttpPut("switch/{id:int:min(1)}")]
        public async Task<Result> SwitchInMenu(int id)
        {
            await _categoryService.SwitchInMenu(id);
            return Result.Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var category = await _categoryRepository.Query().Include(x => x.Children).FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null)
                return Result.Fail("单据不存在");

            if (category.Children.Count > 0)
            {
                return Result.Fail("请确保此类别不包含子类别");
            }

            await _categoryService.Delete(category);
            return Result.Ok();
        }

        [HttpPost]
        public async Task<Result> Create([FromBody]CategoryParam model)
        {
            var category = new Category
            {
                Name = model.Name,
                Slug = model.Slug,
                MetaTitle = model.MetaTitle,
                MetaKeywords = model.MetaKeywords,
                MetaDescription = model.MetaDescription,
                DisplayOrder = model.DisplayOrder,
                Description = model.Description,
                ParentId = model.ParentId,
                IncludeInMenu = model.IncludeInMenu,
                IsPublished = model.IsPublished,
                MediaId = model.MediaId
            };

            await _categoryService.Create(category);
            await _categoryService.ClearCache();
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Update([FromBody]CategoryParam model, int id)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(id);
            if (category == null)
            {
                return Result.Fail("单据不存在");
            }

            category.Name = model.Name;
            category.Slug = model.Slug;
            category.MetaTitle = model.MetaTitle;
            category.MetaKeywords = model.MetaKeywords;
            category.MetaDescription = model.MetaDescription;
            category.Description = model.Description;
            category.DisplayOrder = model.DisplayOrder;
            category.ParentId = model.ParentId;
            category.IncludeInMenu = model.IncludeInMenu;
            category.IsPublished = model.IsPublished;
            category.MediaId = model.MediaId;
            category.UpdatedOn = DateTime.Now;

            if (category.ParentId.HasValue && await HaveCircularNesting(category.Id, category.ParentId.Value))
            {
                return Result.Fail("Parent category cannot be itself children");
            }

            await _categoryService.Update(category);
            await _categoryService.ClearCache();
            return Result.Ok();
        }

        private async Task<bool> HaveCircularNesting(int childId, int parentId)
        {
            if (childId == parentId)
                return true;

            var categories = await _categoryService.GetAllByCache();
            var category = categories.FirstOrDefault(c => c.Id == parentId);
            var parentCategoryId = category?.ParentId;
            while (parentCategoryId.HasValue)
            {
                if (parentCategoryId.Value == childId)
                {
                    return true;
                }
                var parentCategory = categories.FirstOrDefault(c => c.Id == parentCategoryId);
                parentCategoryId = parentCategory?.ParentId;
            }
            return false;
        }
    }
}
