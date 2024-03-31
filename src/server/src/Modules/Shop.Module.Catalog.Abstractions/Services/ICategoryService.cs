using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
{
    /// <summary>
    /// 定义处理商品分类相关操作的服务接口。
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// 获取所有分类。
        /// </summary>
        /// <returns>分类列表。</returns>
        Task<IList<CategoryResult>> GetAll();

        /// <summary>
        /// 通过缓存获取所有分类。
        /// </summary>
        /// <returns>分类列表。</returns>
        Task<IList<Category>> GetAllByCache();

        /// <summary>
        /// 清除分类相关的缓存。
        /// </summary>
        Task ClearCache();

        /// <summary>
        /// 创建新的分类。
        /// </summary>
        /// <param name="category">要创建的分类实体。</param>
        Task Create(Category category);

        /// <summary>
        /// 更新已存在的分类。
        /// </summary>
        /// <param name="category">要更新的分类实体。</param>
        Task Update(Category category);

        /// <summary>
        /// 删除指定的分类。
        /// </summary>
        /// <param name="category">要删除的分类实体。</param>
        Task Delete(Category category);

        /// <summary>
        /// 获取分类列表，支持分页和搜索。
        /// </summary>
        /// <param name="param">分页和搜索参数。</param>
        /// <returns>符合条件的分类列表。</returns>
        Task<Result<StandardTableResult<CategoryResult>>> List(StandardTableParam param);

        /// <summary>
        /// 切换分类在菜单中的显示状态。
        /// </summary>
        /// <param name="id">分类的ID。</param>
        Task SwitchInMenu(int id);

        /// <summary>
        /// 获取指定父分类下的所有子分类。
        /// </summary>
        /// <param name="parentId">父分类的ID。</param>
        /// <param name="all">所有可用的分类列表。</param>
        /// <returns>子分类列表。</returns>
        IList<CategoryResult> GetChildrens(int parentId, IList<CategoryResult> all);

        /// <summary>
        /// 获取一级分类和对应的二级子分类。
        /// </summary>
        /// <param name="parentId">父分类的ID。如果为 null，则获取顶级分类及其子分类。</param>
        /// <param name="isPublished">是否只获取已发布的分类。</param>
        /// <param name="includeInMenu">是否只获取包含在菜单中的分类。</param>
        /// <returns>分类列表。</returns>
        Task<IList<CategoryTwoSubResult>> GetTwoSubCategories(int? parentId = null, bool isPublished = true, bool includeInMenu = true);

        /// <summary>
        /// 仅获取指定父分类的二级子分类。
        /// </summary>
        /// <param name="parentId">父分类的ID。如果为 null，则获取顶级分类的二级子分类。</param>
        /// <param name="isPublished">是否只获取已发布的分类。</param>
        /// <param name="includeInMenu">是否只获取包含在菜单中的分类。</param>
        /// <returns>分类列表。</returns>
        Task<IList<CategoryTwoSubResult>> GetTwoOnlyCategories(int? parentId = null, bool isPublished = true, bool includeInMenu = true);
    }
}
