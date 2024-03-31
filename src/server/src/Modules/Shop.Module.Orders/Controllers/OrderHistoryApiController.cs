using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Orders.Entities;

namespace Shop.Module.Orders.Controllers
{
    /// <summary>
    /// 订单历史 API 控制器，用于管理和查询订单历史记录。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/orders/history")]
    public class OrderHistoryApiController : ControllerBase
    {
        private readonly IRepository<OrderHistory> _orderHistoryRepository;

        public OrderHistoryApiController(IRepository<OrderHistory> orderHistoryRepository)
        {
            _orderHistoryRepository = orderHistoryRepository;
        }

        /// <summary>
        /// 获取指定订单的所有历史记录。
        /// </summary>
        /// <param name="orderId">订单 ID。</param>
        /// <returns>指定订单的历史记录列表。</returns>
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