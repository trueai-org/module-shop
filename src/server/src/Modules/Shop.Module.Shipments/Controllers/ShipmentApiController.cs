using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.Orders.Entities;
using Shop.Module.Shipments.Entities;
using Shop.Module.Shipments.ViewModels;

namespace Shop.Module.Shipments.Controllers
{
    /// <summary>
    /// 发货 API 控制器，负责管理发货单相关操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/shipments")]
    public class ShipmentApiController : ControllerBase
    {
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IWorkContext _workContext;

        public ShipmentApiController(
            IRepository<Shipment> shipmentRepository,
            IRepository<Order> orderRepository,
            IWorkContext workContext)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;
            _workContext = workContext;
        }

        /// <summary>
        /// 获取指定发货单的详细信息。
        /// </summary>
        /// <param name="id">发货单 ID。</param>
        /// <returns>发货单详细信息。</returns>
        [HttpGet("{id}")]
        public async Task<Result<ShipmentQueryResult>> Get(long id)
        {
            var shipment = await _shipmentRepository.Query()
                .Include(c => c.Order)
                .Include(c => c.CreatedBy)
                .Include(c => c.Items).ThenInclude(c => c.OrderItem)
                .Select(c => new ShipmentQueryResult
                {
                    Id = c.Id,
                    AdminComment = c.AdminComment,
                    CreatedBy = c.CreatedBy.FullName,
                    TrackingNumber = c.TrackingNumber,
                    DeliveredOn = c.DeliveredOn,
                    OrderId = c.OrderId,
                    OrderNo = c.Order.No.ToString(),
                    OrderStatus = c.Order.OrderStatus,
                    ShippingStatus = c.Order.ShippingStatus,
                    ShippedOn = c.ShippedOn,
                    TotalWeight = c.TotalWeight,
                    Items = c.Items.Select(x => new ShipmentQueryItemResult()
                    {
                        Id = x.Id,
                        CreatedOn = x.CreatedOn,
                        UpdatedOn = x.UpdatedOn,
                        Quantity = x.Quantity,
                        OrderItemId = x.OrderItemId,
                        ProductId = x.ProductId,
                        ShipmentId = x.ShipmentId,
                        OrderedQuantity = x.OrderItem.Quantity,
                        ProductMediaUrl = x.OrderItem.ProductMediaUrl,
                        ProductName = x.OrderItem.ProductName,
                        ShippedQuantity = x.OrderItem.ShippedQuantity,
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            var currentUser = await _workContext.GetCurrentUserAsync();
            return Result.Ok(shipment);
        }

        /// <summary>
        /// 分页获取所有发货单信息。
        /// </summary>
        /// <param name="param">分页和筛选参数。</param>
        /// <returns>分页的发货单列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<ShipmentQueryResult>>> List([FromBody]StandardTableParam<ShipmentQueryParam> param)
        {
            var query = _shipmentRepository.Query();
            var search = param?.Search;
            if (search != null)
            {
                if (search.OrderNo.HasValue && search.OrderNo.Value > 0)
                {
                    query = query.Where(c => c.Order.No == search.OrderNo.Value);
                }
                if (!string.IsNullOrWhiteSpace(search.TrackingNumber))
                {
                    query = query.Where(c => c.TrackingNumber.Contains(search.TrackingNumber));
                }
                if (search.ShippedOnStart.HasValue)
                    query = query.Where(c => c.CreatedOn >= search.ShippedOnStart.Value);
                if (search.ShippedOnEnd.HasValue)
                    query = query.Where(c => c.CreatedOn < search.ShippedOnEnd.Value);
            }
            var result = await query
                .Include(c => c.Order)
                .Include(c => c.CreatedBy)
                 .Include(c => c.Items).ThenInclude(c => c.OrderItem)
                .ToStandardTableResult(param, c => new ShipmentQueryResult
                {
                    Id = c.Id,
                    AdminComment = c.AdminComment,
                    CreatedBy = c.CreatedBy.FullName,
                    TrackingNumber = c.TrackingNumber,
                    DeliveredOn = c.DeliveredOn,
                    OrderId = c.OrderId,
                    OrderNo = c.Order.No.ToString(),
                    OrderStatus = c.Order.OrderStatus,
                    ShippingStatus = c.Order.ShippingStatus,
                    ShippedOn = c.ShippedOn,
                    TotalWeight = c.TotalWeight,
                    Items = c.Items.Select(x => new ShipmentQueryItemResult()
                    {
                        Id = x.Id,
                        CreatedOn = x.CreatedOn,
                        UpdatedOn = x.UpdatedOn,
                        Quantity = x.Quantity,
                        OrderItemId = x.OrderItemId,
                        ProductId = x.ProductId,
                        ShipmentId = x.ShipmentId,
                        OrderedQuantity = x.OrderItem.Quantity,
                        ProductMediaUrl = x.OrderItem.ProductMediaUrl,
                        ProductName = x.OrderItem.ProductName,
                        ShippedQuantity = x.OrderItem.ShippedQuantity,
                    }).ToList()
                });
            return Result.Ok(result);
        }
    }
}
