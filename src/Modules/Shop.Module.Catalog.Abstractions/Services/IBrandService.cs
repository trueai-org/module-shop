using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Abstractions.Entities;
using Shop.Module.Catalog.Abstractions.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Abstractions.Services
{
    public interface IBrandService
    {
        Task<IList<Brand>> GetAllByCache();

        Task Create(Brand brand);

        Task Update(Brand brand);

        Task<Result<StandardTableResult<BrandResult>>> List(StandardTableParam param);

        Task ClearCache();
    }
}
