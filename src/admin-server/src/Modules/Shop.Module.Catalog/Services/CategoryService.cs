using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Abstractions.Data;
using Shop.Module.Catalog.Abstractions.Entities;
using Shop.Module.Catalog.Abstractions.Services;
using Shop.Module.Catalog.Abstractions.ViewModels;
using Shop.Module.Core.Abstractions.Cache;
using Shop.Module.Core.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly string AllCacheKey = CatalogKeys.CategoryAll;

        private readonly IRepository<Category> _categoryRepository;
        private readonly IStaticCacheManager _cache;
        private readonly IEntityService _entityService;
        public CategoryService(
            IRepository<Category> categoryRepository,
            IStaticCacheManager cache,
            IEntityService entityService)
        {
            _categoryRepository = categoryRepository;
            _cache = cache;
            _entityService = entityService;
        }

        public async Task<IList<CategoryResult>> GetAll()
        {
            var categories = await GetAllByCache();
            var categoriesList = new List<CategoryResult>();
            foreach (var category in categories)
            {
                var categoryListItem = new CategoryResult
                {
                    Id = category.Id,
                    IsPublished = category.IsPublished,
                    IncludeInMenu = category.IncludeInMenu,
                    Name = IncludeParentName(category),
                    DisplayOrder = category.DisplayOrder,
                    ParentId = category.ParentId,
                    CreatedOn = category.CreatedOn,
                    UpdatedOn = category.UpdatedOn
                };
                categoriesList.Add(categoryListItem);
            }
            return categoriesList.OrderBy(x => x.Name).ToList();
        }

        public async Task<IList<Category>> GetAllByCache()
        {
            var result = await _cache.GetAsync(AllCacheKey, async () =>
            {
                return await _categoryRepository.Query().Include(c => c.Media).ToListAsync();
            });
            return result;
        }

        public async Task ClearCache()
        {
            await Task.Run(() =>
            {
                _cache.Remove(AllCacheKey);
            });
        }

        public async Task<Result<StandardTableResult<CategoryResult>>> List(StandardTableParam param)
        {
            var query = _categoryRepository.Query();

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                if (search.name != null)
                {
                    string name = search.name;
                    query = query.Where(x => x.Name.Contains(name));
                }
            }

            var all = await GetAllByCache();

            var gridData = await query.ToStandardTableResult(
                param,
                category => new CategoryResult
                {
                    Id = category.Id,
                    IsPublished = category.IsPublished,
                    IncludeInMenu = category.IncludeInMenu,
                    //Name = IncludeParentName(all.FirstOrDefault(c => c.Id == category.Id)),
                    DisplayOrder = category.DisplayOrder,
                    ParentId = category.ParentId,
                    CreatedOn = category.CreatedOn,
                    UpdatedOn = category.UpdatedOn
                });

            // TODO .NET CORE 3.1
            if (gridData?.List?.Count() > 0)
            {
                gridData.List.ToList().ForEach(c =>
                {
                    c.Name = IncludeParentName(all.FirstOrDefault(x => x.Id == c.Id));
                });
            }

            return Result.Ok(gridData);
        }

        public async Task Create(Category category)
        {
            using (var transaction = _categoryRepository.BeginTransaction())
            {
                //category.Slug = _entityService.ToSafeSlug(category.Slug ?? category.Name, category.Id, EntityTypeWithId.Category);
                _categoryRepository.Add(category);
                await _categoryRepository.SaveChangesAsync();

                //_entityService.Add(category.Name, category.Slug, category.Id, EntityTypeWithId.Category);
                //await _categoryRepository.SaveChangesAsync();

                transaction.Commit();
            }
        }

        public async Task Update(Category category)
        {
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task Delete(Category category)
        {
            category.IsDeleted = true;
            category.UpdatedOn = DateTime.Now;
            await _categoryRepository.SaveChangesAsync();
            await ClearCache();
        }

        private string IncludeParentName(Category category)
        {
            var categoryName = string.Empty;
            if (category == null)
                return categoryName;

            categoryName = category.Name;

            var parentCategory = category.Parent;
            while (parentCategory != null)
            {
                categoryName = $"{parentCategory.Name} >> {categoryName}";
                parentCategory = parentCategory.Parent;
            }

            return categoryName;
        }

        public async Task SwitchInMenu(int id)
        {
            if (id <= 0)
                return;
            var category = _categoryRepository.Query().FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            if (category == null)
                return;

            category.IncludeInMenu = !category.IncludeInMenu;
            category.UpdatedOn = DateTime.Now;
            await _categoryRepository.SaveChangesAsync();
            await ClearCache();
        }

        public IList<CategoryResult> GetChildrens(int id, IList<CategoryResult> all)
        {
            var list = new List<CategoryResult>();
            if (all == null || all.Count <= 0)
                return list;

            var items = all.Where(c => c.ParentId == id);
            list.AddRange(items);

            foreach (var item in items)
            {
                list.AddRange(GetChildrens(item.Id, all));
            }

            return list;
        }

        /// <summary>
        /// 获取一级分类和对应子分类
        /// </summary>
        /// <returns></returns>
        public async Task<IList<CategoryTwoSubResult>> GetTwoSubCategories(int? parentId = null, bool isPublished = true, bool includeInMenu = true)
        {
            var all = await GetAllByCache();
            var list = all.Where(c => c.IsPublished == isPublished && c.IncludeInMenu == includeInMenu && c.ParentId == null);

            if (parentId.HasValue)
            {
                list = list.Where(c => c.Id == parentId);
            }

            var parents = list.Select(c => new CategoryTwoSubResult()
            {
                Id = c.Id,
                ParentId = c.ParentId,
                Name = c.Name,
                DisplayOrder = c.DisplayOrder,
                Slug = c.Slug,
                Description = c.Description,
                ThumbnailUrl = c.Media?.Url
            }).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToList();

            parents.ForEach(x =>
            {
                x.SubCategories = all.Where(c => c.IsPublished == isPublished
                && c.IncludeInMenu == includeInMenu
                && c.ParentId == x.Id).Select(c => new CategoryTwoSubResult()
                {
                    Id = c.Id,
                    ParentId = c.ParentId,
                    Name = c.Name,
                    DisplayOrder = c.DisplayOrder,
                    Slug = c.Slug,
                    Description = c.Description,
                    ThumbnailUrl = c.Media?.Url
                }).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToList();
            });
            return parents;
        }

        /// <summary>
        /// 仅获取二级分类
        /// </summary>
        /// <returns></returns>
        public async Task<IList<CategoryTwoSubResult>> GetTwoOnlyCategories(int? parentId = null, bool isPublished = true, bool includeInMenu = true)
        {
            var list = new List<CategoryTwoSubResult>();
            var twoCategories = await GetTwoSubCategories(parentId, isPublished, includeInMenu);
            foreach (var item in twoCategories.Where(c => c.SubCategories.Count > 0))
            {
                list.AddRange(item.SubCategories);
            }
            return list;
        }
    }
}
