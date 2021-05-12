using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Orders.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Orders.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/orders/history")]
    public class OrderHistoryApiController : ControllerBase
    {
        private readonly IRepository<OrderHistory> _orderHistoryRepository;

        public OrderHistoryApiController(IRepository<OrderHistory> orderHistoryRepository)
        {
            _orderHistoryRepository = orderHistoryRepository;
        }

        [HttpGet("{orderId:int:min(1)}")]
        public async Task<Result> Get(int orderId)
        {
            var histories = await _orderHistoryRepository
                .Query()
                .Include(c => c.CreatedBy)
                .Where(x => x.OrderId == orderId)
                .Select(x => new
                {
                    x.Id,
                    x.OldStatus,
                    x.NewStatus,
                    x.CreatedById,
                    CreatedByFullName = x.CreatedBy.FullName,
                    x.Note,
                    x.CreatedOn
                }).OrderByDescending(c => c.Id).ToListAsync();
            return Result.Ok(histories);
        }
    }
}
