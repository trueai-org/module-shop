using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Cache;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Extensions;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.Inventory.Abstractions.Entities;
using Shop.Module.Orders.Abstractions.Data;
using Shop.Module.Orders.Abstractions.Entities;
using Shop.Module.Orders.Abstractions.Events;
using Shop.Module.Orders.Abstractions.Models;
using Shop.Module.Orders.Abstractions.Services;
using Shop.Module.Orders.Abstractions.ViewModels;
using Shop.Module.Reviews.Abstractions.Entities;
using Shop.Module.Reviews.Abstractions.Models;
using Shop.Module.Reviews.Services.Abstractions;
using Shop.Module.Schedule.Abstractions.Services;
using Shop.Module.Shipments.Abstractions.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Orders.Controllers
{
    [Authorize()]
    [Route("api/customer-orders")]
    public class CustomerOrderApiController : ControllerBase
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserAddress> _userAddressRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<OrderAddress> _orderAddressRepository;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IMediator _mediator;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IRepository<StockHistory> _stockHistoryRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IOrderService _orderService;
        private readonly IUserAddressService _userAddressService;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IAppSettingService _appSettingService;
        private readonly IReviewService _reviewService;
        private readonly IJobService _jobService;
        private readonly ILocker _locker;

        public CustomerOrderApiController(
            IRepository<Order> orderRepository,
            IRepository<User> userRepository,
            IRepository<UserAddress> userAddressRepository,
            IRepository<Product> productRepository,
            IRepository<OrderAddress> orderAddressRepository,
            IWorkContext workContext,
            ICountryService countryService,
            IMediator mediator,
            IRepository<Stock> stockRepository,
            IRepository<StockHistory> stockHistoryRepository,
            IRepository<Shipment> shipmentRepository,
            IOrderService orderService,
            IUserAddressService userAddressService,
            IRepository<Review> reviewRepository,
            IAppSettingService appSettingService,
            IReviewService reviewService,
            IJobService jobService,
            ILocker locker)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _userAddressRepository = userAddressRepository;
            _productRepository = productRepository;
            _orderAddressRepository = orderAddressRepository;
            _workContext = workContext;
            _countryService = countryService;
            _mediator = mediator;
            _stockRepository = stockRepository;
            _stockHistoryRepository = stockHistoryRepository;
            _shipmentRepository = shipmentRepository;
            _orderService = orderService;
            _userAddressService = userAddressService;
            _reviewRepository = reviewRepository;
            _appSettingService = appSettingService;
            _reviewService = reviewService;
            _jobService = jobService;
            _locker = locker;
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var query = _orderRepository.Query()
                .Include(c => c.OrderItems)
                .Include(c => c.Customer)
                .Where(c => c.CustomerId == user.Id && c.Id == id);

            var subQuery = _reviewRepository.Query().Where(c => c.EntityTypeId == (int)EntityTypeWithId.Product && c.SourceId == id && c.SourceType == ReviewSourceType.Order);
            var result = await query.Select(c => new CustomerOrderQueryResult
            {
                Id = c.Id,
                No = c.No.ToString(),
                BillingAddressId = c.BillingAddressId,
                CancelOn = c.CancelOn,
                CancelReason = c.CancelReason,
                CreatedOn = c.CreatedOn,
                CustomerId = c.CustomerId,
                CustomerName = c.Customer.FullName,
                DiscountAmount = c.DiscountAmount,
                OrderNote = c.OrderNote,
                OrderStatus = c.OrderStatus,
                OrderTotal = c.OrderTotal,
                PaymentFeeAmount = c.PaymentFeeAmount,
                PaymentMethod = c.PaymentMethod,
                PaymentOn = c.PaymentOn,
                PaymentType = c.PaymentType,
                ShippingAddressId = c.ShippingAddressId,
                ShippingFeeAmount = c.ShippingFeeAmount,
                ShippingMethod = c.ShippingMethod,
                ShippingStatus = c.ShippingStatus,
                UpdatedOn = c.UpdatedOn,
                Items = c.OrderItems.Select(x => new CustomerOrderItemQueryResult
                {
                    OrderId = x.OrderId,
                    DiscountAmount = x.DiscountAmount,
                    ItemAmount = x.ItemAmount,
                    ItemWeight = x.ItemWeight,
                    Note = x.Note,
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductMediaUrl = x.ProductMediaUrl,
                    ProductName = x.ProductName,
                    ProductPrice = x.ProductPrice,
                    Quantity = x.Quantity,
                    ShippedQuantity = x.ShippedQuantity,
                    IsReviewed = subQuery.Any(y => y.EntityId == x.ProductId)
                }),
                ItemsTotal = c.OrderItems.Sum(x => x.Quantity),
                ItemsCount = c.OrderItems.Count,
                PaymentEndOn = c.PaymentEndOn,
                DeliveredEndOn = c.DeliveredEndOn
            }).FirstOrDefaultAsync();

            if (result?.ShippingAddressId != null)
            {
                result.Address = await _orderService.GetOrderAddress(result.ShippingAddressId.Value);
            }
            return Result.Ok(result);
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<CustomerOrderQueryResult>>> List([FromBody] StandardTableParam<CustomerOrderQueryParam> param)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var query = _orderRepository.Query()
                .Include(c => c.OrderItems)
                .Include(c => c.Customer)
                .Where(c => c.CustomerId == user.Id);
            var search = param.Search;
            if (search != null)
            {
                if (search.OrderStatus != null && search.OrderStatus.Count > 0)
                    query = query.Where(c => search.OrderStatus.Contains(c.OrderStatus));

                if (search.ShippingStatus.HasValue)
                    query = query.Where(c => c.ShippingStatus == search.ShippingStatus.Value);
            }

            var result = await query.ToStandardTableResult(param,
                c => new CustomerOrderQueryResult
                {
                    Id = c.Id,
                    No = c.No.ToString(),
                    BillingAddressId = c.BillingAddressId,
                    CancelOn = c.CancelOn,
                    CancelReason = c.CancelReason,
                    CreatedOn = c.CreatedOn,
                    CustomerId = c.CustomerId,
                    CustomerName = c.Customer.FullName,
                    DiscountAmount = c.DiscountAmount,
                    OrderNote = c.OrderNote,
                    OrderStatus = c.OrderStatus,
                    OrderTotal = c.OrderTotal,
                    PaymentFeeAmount = c.PaymentFeeAmount,
                    PaymentMethod = c.PaymentMethod,
                    PaymentOn = c.PaymentOn,
                    PaymentType = c.PaymentType,
                    ShippingAddressId = c.ShippingAddressId,
                    ShippingFeeAmount = c.ShippingFeeAmount,
                    ShippingMethod = c.ShippingMethod,
                    ShippingStatus = c.ShippingStatus,
                    UpdatedOn = c.UpdatedOn,
                    Items = c.OrderItems.Select(x => new CustomerOrderItemQueryResult
                    {
                        OrderId = x.OrderId,
                        DiscountAmount = x.DiscountAmount,
                        ItemAmount = x.ItemAmount,
                        ItemWeight = x.ItemWeight,
                        Note = x.Note,
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductMediaUrl = x.ProductMediaUrl,
                        ProductName = x.ProductName,
                        ProductPrice = x.ProductPrice,
                        Quantity = x.Quantity,
                        ShippedQuantity = x.ShippedQuantity
                    }),// EF CORE 子查询列表 top bug, 不允许此操作.Take(2), // 列表中最多显示2个商品
                    ItemsTotal = c.OrderItems.Sum(x => x.Quantity),
                    ItemsCount = c.OrderItems.Count,
                    PaymentEndOn = c.PaymentEndOn,
                    DeliveredEndOn = c.DeliveredEndOn
                });

            if (result.List?.Count() > 0)
            {
                result.List.ToList().ForEach(c =>
                {
                    if (c.Items?.Count() > 2)
                    {
                        c.Items = c.Items.Take(2).ToList();
                    }
                });
            }
            return Result.Ok(result);
        }

        [HttpPut("{id:int:min(1)}/cancel")]
        public async Task<Result> Cancel(int id, [FromBody] OrderCancelParam reason)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var order = await _orderRepository.Query()
                .FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.Id == id);
            if (order == null)
            {
                return Result.Fail("订单不存在");
            }
            else if (order.OrderStatus == OrderStatus.Canceled)
            {
                return Result.Fail("订单已取消");
            }
            var orderSs = new OrderStatus[] { OrderStatus.New, OrderStatus.PendingPayment, OrderStatus.PaymentFailed };
            if (!orderSs.Contains(order.OrderStatus))
            {
                return Result.Fail("当前订单无法取消"); ;
            }
            await _orderService.Cancel(id, user.Id, reason?.Reason);
            return Result.Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var currentUser = await _workContext.GetCurrentOrThrowAsync();
            var order = await _orderRepository
                .Query()
                .Include(c => c.BillingAddress)
                .Include(c => c.ShippingAddress)
                .Include(c => c.OrderItems).ThenInclude(c => c.Product)
                .Where(c => c.Id == id && c.CustomerId == currentUser.Id).FirstOrDefaultAsync();
            if (order == null)
            {
                return Result.Fail("订单不存在");
            }
            var orderSs = new OrderStatus[] { OrderStatus.Complete, OrderStatus.Canceled };
            if (!orderSs.Contains(order.OrderStatus))
            {
                return Result.Fail("当前订单状态不允许删除");
            }

            if (order.ShippingAddress != null)
            {
                order.ShippingAddress.IsDeleted = true;
                order.ShippingAddress.UpdatedOn = DateTime.Now;
            }
            if (order.BillingAddress != null)
            {
                order.BillingAddress.IsDeleted = true;
                order.BillingAddress.UpdatedOn = DateTime.Now;
            }

            foreach (var item in order.OrderItems)
            {
                item.IsDeleted = true;
                item.UpdatedOn = DateTime.Now;
                item.UpdatedBy = currentUser;
            }

            //删除订单暂不删除历史

            order.IsDeleted = true;
            order.UpdatedOn = DateTime.Now;
            order.UpdatedBy = currentUser;

            await _orderRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}/cinfirm-receipt")]
        public async Task<Result> CinfirmReceipt(int id)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var order = await _orderRepository.Query()
                .FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.Id == id);
            if (order == null)
            {
                return Result.Fail("订单不存在");
            }
            else if (order.OrderStatus == OrderStatus.Canceled)
            {
                return Result.Fail("订单已取消");
            }
            var orderSs = new OrderStatus[] { OrderStatus.PaymentReceived, OrderStatus.Shipping, OrderStatus.Shipped };
            if (!orderSs.Contains(order.OrderStatus))
            {
                return Result.Fail("当前订单无法确认收货"); ;
            }

            order.OrderStatus = OrderStatus.Complete;
            order.ShippingStatus = ShippingStatus.Delivered;
            order.DeliveredOn = DateTime.Now;
            order.UpdatedOn = DateTime.Now;
            order.UpdatedById = user.Id;
            await _orderRepository.SaveChangesAsync();

            // 自动好评
            var min = await _appSettingService.Get<int>(OrderKeys.OrderCompleteAutoReviewTimeForMinute);
            foreach (var item in order.OrderItems)
            {
                await _jobService.Schedule(() =>
                _reviewService.ReviewAutoGood(item.ProductId, EntityTypeWithId.Product, order.Id, ReviewSourceType.Order)
                , TimeSpan.FromMinutes(min));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 买家延长确认收货+7天，最大不超过60d
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:int:min(1)}/delay-cinfirm-receipt")]
        public async Task<Result> DelayCinfirmReceipt(int id)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var order = await _orderRepository.Query()
                .FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.Id == id);
            if (order == null)
            {
                return Result.Fail("订单不存在");
            }
            else if (order.OrderStatus == OrderStatus.Canceled)
            {
                return Result.Fail("订单已取消");
            }
            var orderSs = new OrderStatus[] { OrderStatus.PaymentReceived, OrderStatus.Shipping, OrderStatus.Shipped };
            if (!orderSs.Contains(order.OrderStatus))
            {
                return Result.Fail("当前订单无法延长确认收货时间"); ;
            }

            var timeFromMin = await _appSettingService.Get<int>(OrderKeys.OrderAutoCompleteTimeForMinute);
            if (order.DeliveredEndOn == null)
            {
                order.DeliveredEndOn = DateTime.Now;
            }
            order.DeliveredEndOn = order.DeliveredEndOn.Value.AddMinutes(timeFromMin);

            if (order.DeliveredEndOn > order.CreatedOn.AddDays(60))
            {
                return Result.Fail("延长确认收货时间最大不可超过60天");
            }

            order.UpdatedOn = DateTime.Now;
            order.UpdatedById = user.Id;
            await _orderRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpGet("{id:int:min(1)}/pay")]
        public async Task<Result> PayInfo(int id)
        {
            var result = await _orderService.PayInfo(id);
            return Result.Ok(result);
        }
    }
}
