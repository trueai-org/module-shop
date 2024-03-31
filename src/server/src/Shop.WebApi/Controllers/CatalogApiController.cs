using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Catalog.Services;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers
{
    /// <summary>
    /// 商品目录 API 控制器，提供商品目录相关的 API 接口。
    /// </summary>
    [ApiController]
    [Route("api/catalogs")]
    public class CatalogApiController
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// 构造函数，注入商品类别服务接口。
        /// </summary>
        /// <param name="categoryService">商品类别服务接口。</param>
        public CatalogApiController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// 获取两个子类别的信息。
        /// </summary>
        /// <returns>操作结果，包含两个子类别的信息。</returns>
        [HttpGet()]
        public async Task<Result> GetTwoSubCategories()
        {
            var result = await _categoryService.GetTwoSubCategories();
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据父类别 ID 获取其下只有两个子类别的信息。
        /// </summary>
        /// <param name="parentId">父类别的 ID，如果为空则获取最顶层类别。</param>
        /// <returns>操作结果，包含子类别的信息。</returns>
        [HttpGet("sub-categories")]
        public async Task<Result> GetTwoOnlyCategories(int? parentId = null)
        {
            var result = await _categoryService.GetTwoOnlyCategories(parentId);
            return Result.Ok(result);
        }
    }
}
