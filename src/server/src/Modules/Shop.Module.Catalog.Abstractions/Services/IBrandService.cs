using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
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
