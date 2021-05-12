using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Catalog.Services;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers
{
    [ApiController]
    [Route("api/catalogs")]
    public class CatalogApiController
    {
        private readonly ICategoryService _categoryService;

        public CatalogApiController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet()]
        public async Task<Result> GetTwoSubCategories()
        {
            var result = await _categoryService.GetTwoSubCategories();
            return Result.Ok(result);
        }

        [HttpGet("sub-categories")]
        public async Task<Result> GetTwoOnlyCategories(int? parentId = null)
        {
            var result = await _categoryService.GetTwoOnlyCategories(parentId);
            return Result.Ok(result);
        }
    }
}
