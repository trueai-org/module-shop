using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Inventory.Entities;
using Shop.Module.Inventory.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Inventory.Areas.Inventory.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/api/stocks-histories")]
    public class StockHistoryApiController : ControllerBase
    {
        private readonly IRepository<StockHistory> _stockHistoryRepository;
        public StockHistoryApiController(IRepository<StockHistory> stockHistoryRepository)
        {
            _stockHistoryRepository = stockHistoryRepository;
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<StockHistoryQueryResult>>> List([FromBody]StandardTableParam<StockHistoryQueryParam> param)
        {
            var query = _stockHistoryRepository.Query();
            var warehouseId = param?.Search?.WarehouseId;
            var productId = param?.Search?.ProductId;
            if (warehouseId.HasValue)
                query = query.Where(c => c.WarehouseId == warehouseId.Value);
            if (productId.HasValue)
                query = query.Where(c => c.ProductId == productId.Value);

            var result = await query.Include(c => c.Warehouse)
                .ToStandardTableResult(param, c => new StockHistoryQueryResult
                {
                    Id = c.Id,
                    WarehouseId = c.WarehouseId,
                    AdjustedQuantity = c.AdjustedQuantity,
                    CreatedById = c.CreatedById,
                    CreatedOn = c.CreatedOn,
                    Note = c.Note,
                    ProductId = c.ProductId,
                    StockQuantity = c.StockQuantity,
                    WarehouseName = c.Warehouse.Name
                });
            return Result.Ok(result);
        }
    }
}
