using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
{
    public interface ICategoryService
    {
        Task<IList<CategoryResult>> GetAll();

        Task<IList<Category>> GetAllByCache();

        Task ClearCache();

        Task Create(Category category);

        Task Update(Category category);

        Task Delete(Category category);

        Task<Result<StandardTableResult<CategoryResult>>> List(StandardTableParam param);

        Task SwitchInMenu(int id);

        IList<CategoryResult> GetChildrens(int parentId, IList<CategoryResult> all);

        /// <summary>
        /// 获取一级分类和对应子分类
        /// </summary>
        /// <returns></returns>
        Task<IList<CategoryTwoSubResult>> GetTwoSubCategories(int? parentId = null, bool isPublished = true, bool includeInMenu = true);

        /// <summary>
        /// 仅获取二级分类
        /// </summary>
        /// <returns></returns>
        Task<IList<CategoryTwoSubResult>> GetTwoOnlyCategories(int? parentId = null, bool isPublished = true, bool includeInMenu = true);
    }
}
