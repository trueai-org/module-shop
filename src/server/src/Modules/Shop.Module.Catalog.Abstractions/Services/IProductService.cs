using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services
{
    public interface IProductService
    {
        Task<StandardTableResult<GoodsListResult>> HomeList(StandardTableParam<GoodsListQueryParam> param);

        Task<GoodsGetResult> GetGoodsByCache(int id);

        Task ClearGoodsCacheAndParent(int id);

        Task<IList<GoodsListResult>> RelatedList(int id);

        Task<IList<GoodsGetStockResult>> GetGoodsStocks(int id);
    }
}
