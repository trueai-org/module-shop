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

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 商品分类API控制器，负责商品分类的管理操作，如查询、创建、更新和删除。
    /// </summary>
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


        /// <summary>
        /// 获取所有商品分类的信息。
        /// </summary>
        /// <returns>返回所有商品分类的信息列表。</returns>
        [HttpGet]
        public async Task<Result<IList<CategoryResult>>> Get()
        {
            var result = await _categoryService.GetAll();
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据商品分类ID获取指定商品分类的详细信息。
        /// </summary>
        /// <param name="id">商品分类ID。</param>
        /// <returns>返回指定商品分类的详细信息。</returns>
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

        /// <summary>
        /// 分页获取商品分类列表。
        /// </summary>
        /// <param name="param">包含分页和排序参数的对象。</param>
        /// <returns>返回分页的商品分类列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<CategoryResult>>> List([FromBody] StandardTableParam param)
        {
            var result = await _categoryService.List(param);
            return result;
        }

        /// <summary>
        /// 清除商品分类的缓存。
        /// </summary>
        /// <returns>返回操作结果。</returns>
        [HttpPost("clear-cache")]
        public async Task<Result> ClearCache()
        {
            await _categoryService.ClearCache();
            return Result.Ok();
        }

        /// <summary>
        /// 切换指定商品分类在菜单中的显示状态。
        /// </summary>
        /// <param name="id">商品分类ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("switch/{id:int:min(1)}")]
        public async Task<Result> SwitchInMenu(int id)
        {
            await _categoryService.SwitchInMenu(id);
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定ID的商品分类。
        /// </summary>
        /// <param name="id">商品分类ID。</param>
        /// <returns>返回操作结果。</returns>
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

        /// <summary>
        /// 创建新的商品分类。
        /// </summary>
        /// <param name="model">包含商品分类信息的参数对象。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPost]
        public async Task<Result> Create([FromBody] CategoryParam model)
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

        /// <summary>
        /// 更新指定ID的商品分类信息。
        /// </summary>
        /// <param name="model">包含商品分类更新信息的参数对象。</param>
        /// <param name="id">商品分类ID。</param>
        /// <returns>返回操作结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Update([FromBody] CategoryParam model, int id)
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