using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Inventory.Entities;
using Shop.Module.Orders.Data;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Models;
using Shop.Module.Orders.Services;
using Shop.Module.Orders.ViewModels;
using Shop.Module.Shipments.Entities;

namespace Shop.Module.Orders.Controllers
{
    /// <summary>
    /// 管理员订单 API 控制器，处理订单的管理和操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/orders")]
    public class OrderApiController : ControllerBase
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
        private readonly IAppSettingService _appSettingService;

        public OrderApiController(
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
            IAppSettingService appSettingService)
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
            _appSettingService = appSettingService;
        }

        /// <summary>
        /// 根据订单 ID 获取订单详细信息。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <returns>订单详细信息。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result<OrderGetResult>> Get(int id)
        {
            var result = await GetOrder(id);
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据订单号获取订单详细信息。
        /// </summary>
        /// <param name="no">订单号。</param>
        /// <returns>订单详细信息。</returns>
        [HttpGet("{no:long:min(1)}/no")]
        public async Task<Result<OrderGetResult>> GetByNo(long no)
        {
            var result = await GetOrder(0, no);
            return Result.Ok(result);
        }

        /// <summary>
        /// 创建新订单。
        /// </summary>
        /// <param name="model">订单创建参数。</param>
        /// <returns>创建操作的结果。</returns>
        [HttpPost]
        public async Task<Result> Post([FromBody] OrderCreateParam model)
        {
            if (model == null)
                throw new Exception("参数异常");
            if (model.Items == null || model.Items.Count <= 0)
                throw new Exception("请添加商品");
            if (model.Items.Any(c => c.Quantity <= 0))
                throw new Exception("购买商品数量必须>0");

            var user = await _workContext.GetCurrentUserAsync();

            var customer = await _userRepository.FirstOrDefaultAsync(model.CustomerId);
            if (customer == null)
                throw new Exception("客户不存在");

            var order = new Order()
            {
                OrderStatus = OrderStatus.New,
                CreatedBy = user,
                UpdatedBy = user,
                CustomerId = model.CustomerId,
                AdminNote = model.AdminNote,
                OrderNote = model.OrderNote,
                ShippingMethod = model.ShippingMethod,
                PaymentType = model.PaymentType,
                ShippingFeeAmount = model.ShippingFeeAmount,
                OrderTotal = model.OrderTotal,
                DiscountAmount = model.DiscountAmount,
            };

            OrderAddress orderShipping = null;
            OrderAddress orderBilling = null;
            if (model.ShippingUserAddressId.HasValue && model.ShippingUserAddressId.Value > 0)
            {
                orderShipping = await this.UserAddressToOrderAddress(model.ShippingUserAddressId.Value, customer.Id, AddressType.Shipping, order);
            }
            if (model.BillingUserAddressId.HasValue && model.BillingUserAddressId.Value > 0)
            {
                orderBilling = await this.UserAddressToOrderAddress(model.BillingUserAddressId.Value, customer.Id, AddressType.Billing, order);
            }

            if (model.ShippingAddress != null && orderShipping == null)
            {
                orderShipping = new OrderAddress()
                {
                    Order = order,
                    AddressType = AddressType.Shipping,
                    AddressLine1 = model.ShippingAddress.AddressLine1,
                    AddressLine2 = model.ShippingAddress.AddressLine2,
                    City = model.ShippingAddress.City,
                    Company = model.ShippingAddress.Company,
                    ContactName = model.ShippingAddress.ContactName,
                    CountryId = model.ShippingAddress.CountryId,
                    Email = model.ShippingAddress.Email,
                    Phone = model.ShippingAddress.Phone,
                    StateOrProvinceId = model.ShippingAddress.StateOrProvinceId,
                    ZipCode = model.ShippingAddress.ZipCode
                };
            }
            if (model.BillingAddress != null && orderBilling == null)
            {
                orderBilling = new OrderAddress()
                {
                    Order = order,
                    AddressType = AddressType.Billing,
                    AddressLine1 = model.BillingAddress.AddressLine1,
                    AddressLine2 = model.BillingAddress.AddressLine2,
                    City = model.BillingAddress.City,
                    Company = model.BillingAddress.Company,
                    ContactName = model.BillingAddress.ContactName,
                    CountryId = model.BillingAddress.CountryId,
                    Email = model.BillingAddress.Email,
                    Phone = model.BillingAddress.Phone,
                    StateOrProvinceId = model.BillingAddress.StateOrProvinceId,
                    ZipCode = model.BillingAddress.ZipCode
                };
            }

            var productIds = model.Items.Select(c => c.Id).Distinct();
            var products = await _productRepository.Query()
                .Include(c => c.ThumbnailImage)
                .Where(c => productIds.Contains(c.Id)).ToListAsync();

            if (productIds.Count() <= 0)
                throw new Exception("商品不存在");

            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var addStockHistories = new List<StockHistory>();
            foreach (var item in products)
            {
                var first = model.Items.FirstOrDefault(c => c.Id == item.Id);
                if (first == null)
                    throw new Exception($"产品[{item.Name}]不存在");

                if (!item.IsPublished)
                    throw new Exception($"产品[{item.Name}]未发布");
                if (!item.IsAllowToOrder)
                    throw new Exception($"产品[{item.Name}]不允许购买");

                OrderStockDoWorker(stocks, addStockHistories, item, user, -first.Quantity, order, "创建订单");

                var orderItem = new OrderItem()
                {
                    Order = order,
                    Product = item,
                    ItemWeight = 0,
                    ItemAmount = first.Quantity * first.ProductPrice - first.DiscountAmount,
                    Quantity = first.Quantity,
                    ProductPrice = first.ProductPrice,
                    DiscountAmount = first.DiscountAmount,
                    CreatedBy = user,
                    UpdatedBy = user,
                    ProductName = item.Name,
                    ProductMediaUrl = item.ThumbnailImage?.Url
                };
                order.OrderItems.Add(orderItem);
            }

            order.SubTotal = order.OrderItems.Sum(c => c.Quantity * c.ProductPrice);
            order.SubTotalWithDiscount = order.OrderItems.Sum(c => c.DiscountAmount);
            _orderRepository.Add(order);

            // Unable to save changes because a circular dependency was detected in the data to be saved
            // https://github.com/aspnet/EntityFrameworkCore/issues/11888
            // https://docs.microsoft.com/zh-cn/ef/core/saving/transactions
            // https://stackoverflow.com/questions/40073149/entity-framework-circular-dependency-for-last-entity
            using (var transaction = _orderRepository.BeginTransaction())
            {
                await _orderRepository.SaveChangesAsync();

                order.ShippingAddress = orderShipping;
                order.BillingAddress = orderBilling;
                await _orderRepository.SaveChangesAsync();

                var orderCreated = new OrderCreated
                {
                    OrderId = order.Id,
                    Order = order,
                    UserId = order.CreatedById,
                    Note = order.OrderNote
                };
                await _mediator.Publish(orderCreated);

                await _stockRepository.SaveChangesAsync();
                if (addStockHistories.Count > 0)
                {
                    _stockHistoryRepository.AddRange(addStockHistories);
                    await _stockHistoryRepository.SaveChangesAsync();
                }
                transaction.Commit();
            }

            //TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
            //using (var ts = new TransactionScope())
            //{
            //    ts.Complete();
            //}
            return Result.Ok();
        }

        /// <summary>
        /// 更新订单信息。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <param name="model">订单编辑参数。</param>
        /// <returns>更新操作的结果。</returns>
        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody] OrderEditParam model)
        {
            if (model == null)
                throw new Exception("参数异常");
            if (model.Items == null || model.Items.Count <= 0)
                throw new Exception("请添加商品");
            if (model.Items.Any(c => c.Quantity <= 0))
                throw new Exception("购买商品数量必须>0");

            var currentUser = await _workContext.GetCurrentUserAsync();
            var order = await _orderRepository
                .Query()
                .Include(c => c.Customer)
                .Include(c => c.BillingAddress)
                .Include(c => c.ShippingAddress)
                .Include(c => c.OrderItems)
                .Where(c => c.Id == id).FirstOrDefaultAsync();
            if (order == null)
                throw new Exception("订单不存在");

            var user = await _workContext.GetCurrentUserAsync();
            var oldStatus = order.OrderStatus;

            order.ShippingAddressId = model.ShippingAddressId;
            order.BillingAddressId = model.BillingAddressId;

            if (model.ShippingAddress != null)
            {
                if (order.ShippingAddress == null)
                {
                    order.ShippingAddress = new OrderAddress()
                    {
                        Order = order,
                        AddressType = AddressType.Shipping,
                        AddressLine1 = model.ShippingAddress.AddressLine1,
                        AddressLine2 = model.ShippingAddress.AddressLine2,
                        City = model.ShippingAddress.City,
                        Company = model.ShippingAddress.Company,
                        ContactName = model.ShippingAddress.ContactName,
                        CountryId = model.ShippingAddress.CountryId,
                        Email = model.ShippingAddress.Email,
                        Phone = model.ShippingAddress.Phone,
                        StateOrProvinceId = model.ShippingAddress.StateOrProvinceId,
                        ZipCode = model.ShippingAddress.ZipCode
                    };
                }
                else
                {
                    order.ShippingAddress.AddressType = AddressType.Shipping;
                    order.ShippingAddress.AddressLine1 = model.ShippingAddress.AddressLine1;
                    order.ShippingAddress.AddressLine2 = model.ShippingAddress.AddressLine2;
                    order.ShippingAddress.City = model.ShippingAddress.City;
                    order.ShippingAddress.Company = model.ShippingAddress.Company;
                    order.ShippingAddress.ContactName = model.ShippingAddress.ContactName;
                    order.ShippingAddress.CountryId = model.ShippingAddress.CountryId;
                    order.ShippingAddress.Email = model.ShippingAddress.Email;
                    order.ShippingAddress.Phone = model.ShippingAddress.Phone;
                    order.ShippingAddress.StateOrProvinceId = model.ShippingAddress.StateOrProvinceId;
                    order.ShippingAddress.ZipCode = model.ShippingAddress.ZipCode;
                }
            }

            if (model.BillingAddress != null)
            {
                if (order.BillingAddress == null)
                {
                    order.BillingAddress = new OrderAddress()
                    {
                        Order = order,
                        AddressType = AddressType.Shipping,
                        AddressLine1 = model.BillingAddress.AddressLine1,
                        AddressLine2 = model.BillingAddress.AddressLine2,
                        City = model.BillingAddress.City,
                        Company = model.BillingAddress.Company,
                        ContactName = model.BillingAddress.ContactName,
                        CountryId = model.BillingAddress.CountryId,
                        Email = model.BillingAddress.Email,
                        Phone = model.BillingAddress.Phone,
                        StateOrProvinceId = model.BillingAddress.StateOrProvinceId,
                        ZipCode = model.BillingAddress.ZipCode
                    };
                }
                else
                {
                    order.BillingAddress.AddressType = AddressType.Shipping;
                    order.BillingAddress.AddressLine1 = model.BillingAddress.AddressLine1;
                    order.BillingAddress.AddressLine2 = model.BillingAddress.AddressLine2;
                    order.BillingAddress.City = model.BillingAddress.City;
                    order.BillingAddress.Company = model.BillingAddress.Company;
                    order.BillingAddress.ContactName = model.BillingAddress.ContactName;
                    order.BillingAddress.CountryId = model.BillingAddress.CountryId;
                    order.BillingAddress.Email = model.BillingAddress.Email;
                    order.BillingAddress.Phone = model.BillingAddress.Phone;
                    order.BillingAddress.StateOrProvinceId = model.BillingAddress.StateOrProvinceId;
                    order.BillingAddress.ZipCode = model.BillingAddress.ZipCode;
                }
            }

            var productIds = model.Items.Select(c => c.Id).Distinct();
            var products = await _productRepository.Query()
                .Include(c => c.ThumbnailImage)
                .Where(c => productIds.Contains(c.Id)).ToListAsync();
            if (productIds.Count() <= 0)
                throw new Exception("商品不存在");

            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var addStockHistories = new List<StockHistory>();
            foreach (var item in products)
            {
                var first = model.Items.FirstOrDefault(c => c.Id == item.Id);
                if (first == null)
                    throw new Exception($"产品[{item.Name}]不存在");

                if (!item.IsPublished)
                    throw new Exception($"产品[{item.Name}]未发布");
                if (!item.IsAllowToOrder)
                    throw new Exception($"产品[{item.Name}]不允许购买");

                var productStocks = stocks.Where(c => c.ProductId == item.Id);

                if (order.OrderItems.Any(c => c.ProductId == item.Id))
                {
                    var orderItem = order.OrderItems.First(c => c.ProductId == item.Id);

                    OrderStockDoWorker(stocks, addStockHistories, item, user, orderItem.Quantity - first.Quantity, order, "修改下单商品数量");

                    orderItem.ItemWeight = 0;
                    orderItem.ItemAmount = first.Quantity * first.ProductPrice - first.DiscountAmount;
                    orderItem.Quantity = first.Quantity;
                    orderItem.ProductPrice = first.ProductPrice;
                    orderItem.DiscountAmount = first.DiscountAmount;
                    orderItem.UpdatedBy = user;
                    orderItem.ProductName = item.Name;
                    orderItem.ProductMediaUrl = item.ThumbnailImage?.Url;
                    orderItem.UpdatedOn = DateTime.Now;
                }
                else
                {
                    OrderStockDoWorker(stocks, addStockHistories, item, user, -first.Quantity, order, "修改订单，增加商品");

                    var orderItem = new OrderItem()
                    {
                        Order = order,
                        Product = item,
                        ItemWeight = 0,
                        ItemAmount = first.Quantity * first.ProductPrice - first.DiscountAmount,
                        Quantity = first.Quantity,
                        ProductPrice = first.ProductPrice,
                        DiscountAmount = first.DiscountAmount,
                        CreatedBy = user,
                        UpdatedBy = user,
                        ProductName = item.Name,
                        ProductMediaUrl = item.ThumbnailImage?.Url
                    };
                    order.OrderItems.Add(orderItem);
                }
            }
            var deletedProductItems = order.OrderItems.Where(c => !productIds.Contains(c.ProductId));
            foreach (var item in deletedProductItems)
            {
                item.IsDeleted = true;
                item.UpdatedOn = DateTime.Now;
            }

            order.OrderStatus = model.OrderStatus;
            order.DiscountAmount = model.DiscountAmount;
            order.OrderTotal = model.OrderTotal;
            order.OrderNote = model.OrderNote;
            order.AdminNote = model.AdminNote;
            order.PaymentType = model.PaymentType;
            order.ShippingFeeAmount = model.ShippingFeeAmount;
            order.ShippingMethod = model.ShippingMethod;
            order.ShippingStatus = model.ShippingStatus;
            order.PaymentMethod = model.PaymentMethod;
            order.PaymentFeeAmount = model.PaymentFeeAmount;
            order.PaymentOn = model.PaymentOn;
            order.ShippedOn = model.ShippedOn;
            order.DeliveredOn = model.DeliveredOn;
            order.CancelOn = model.CancelOn;
            order.CancelReason = model.CancelReason;
            order.RefundAmount = model.RefundAmount;
            order.RefundOn = model.RefundOn;
            order.RefundReason = model.RefundReason;
            order.RefundStatus = model.RefundStatus;

            order.SubTotal = order.OrderItems.Sum(c => c.Quantity * c.ProductPrice);
            order.SubTotalWithDiscount = order.OrderItems.Sum(c => c.DiscountAmount);

            using (var transaction = _orderRepository.BeginTransaction())
            {
                await _orderRepository.SaveChangesAsync();

                var orderStatusChanged = new OrderChanged
                {
                    OrderId = order.Id,
                    OldStatus = oldStatus,
                    NewStatus = order.OrderStatus,
                    Order = order,
                    UserId = currentUser.Id,
                    Note = model.AdminNote
                };
                await _mediator.Publish(orderStatusChanged);

                await _stockRepository.SaveChangesAsync();
                if (addStockHistories.Count > 0)
                {
                    _stockHistoryRepository.AddRange(addStockHistories);
                    await _stockHistoryRepository.SaveChangesAsync();
                }
                transaction.Commit();
            }
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定订单。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <returns>删除操作的结果。</returns>
        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var order = await _orderRepository
                .Query()
                .Include(c => c.BillingAddress)
                .Include(c => c.ShippingAddress)
                .Include(c => c.OrderItems).ThenInclude(c => c.Product)
                .Where(c => c.Id == id).FirstOrDefaultAsync();
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

        /// <summary>
        /// 取消指定订单。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <param name="reason">取消订单的原因。</param>
        /// <returns>取消操作的结果。</returns>
        [HttpPut("{id:int:min(1)}/cancel")]
        public async Task<Result> Cancel(int id, [FromBody] OrderCancelParam reason)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            await _orderService.Cancel(id, currentUser.Id, reason?.Reason);
            return Result.Ok();
        }

        /// <summary>
        /// 将指定订单挂起。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <param name="param">挂起订单的参数。</param>
        /// <returns>挂起操作的结果。</returns>
        [HttpPut("{id:int:min(1)}/on-hold")]
        public async Task<Result> OnHold(int id, [FromBody] OrderOnHoldParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var order = await _orderRepository
                .Query()
                .Where(c => c.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return Result.Fail("订单不存在");

            if (order.OrderStatus == OrderStatus.OnHold)
                return Result.Fail("订单已挂起");

            var orderNotSs = new OrderStatus[] { OrderStatus.Canceled, OrderStatus.Complete };
            if (orderNotSs.Contains(order.OrderStatus))
                return Result.Fail("当前订单状态不允许挂起");

            var oldStatus = order.OrderStatus;

            order.OrderStatus = OrderStatus.OnHold;
            order.OnHoldReason = param?.Reason;
            order.UpdatedOn = DateTime.Now;
            order.UpdatedBy = currentUser;
            await _orderRepository.SaveChangesAsync();

            var orderStatusChanged = new OrderChanged
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = order.OrderStatus,
                Order = order,
                UserId = currentUser.Id,
                Note = "挂起订单"
            };
            await _mediator.Publish(orderStatusChanged);

            return Result.Ok();
        }

        /// <summary>
        /// 标记指定订单为已支付。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <returns>标记操作的结果。</returns>
        [HttpPut("{id:int:min(1)}/payment")]
        public async Task<Result> AdminPayment(int id)
        {
            var currentUser = await _workContext.GetCurrentOrThrowAsync();
            var order = await _orderRepository.Query().Where(c => c.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return Result.Fail("订单不存在");

            var orderSs = new OrderStatus[] { OrderStatus.New, OrderStatus.PendingPayment, OrderStatus.PaymentFailed };
            if (!orderSs.Contains(order.OrderStatus))
            {
                return Result.Fail("当前订单状态不允许标记付款");
            }

            await _orderService.PaymentReceived(new PaymentReceivedParam()
            {
                OrderId = order.Id,
                Note = "标记付款"
            });

            return Result.Ok();
        }

        /// <summary>
        /// 分页获取订单列表。
        /// </summary>
        /// <param name="param">分页查询参数。</param>
        /// <returns>分页的订单列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<OrderQueryResult>>> List([FromBody] StandardTableParam<OrderQueryParam> param)
        {
            var query = _orderRepository.Query();
            var search = param.Search;
            if (search != null)
            {
                if (search.CustomerId.HasValue)
                    query = query.Where(c => c.CustomerId == search.CustomerId.Value);
                if (search.CreatedOnStart.HasValue)
                    query = query.Where(c => c.CreatedOn >= search.CreatedOnStart.Value);
                if (search.CreatedOnEnd.HasValue)
                    query = query.Where(c => c.CreatedOn < search.CreatedOnEnd.Value.AddDays(1));
                if (search.OrderStatus.Count > 0)
                    query = query.Where(c => search.OrderStatus.Contains(c.OrderStatus));
                if (search.ShippingStatus.Count > 0)
                    query = query.Where(c => search.ShippingStatus.Contains(c.ShippingStatus.Value));

                if (!string.IsNullOrWhiteSpace(search.ProductName))
                {
                    query = query.Where(c => c.OrderItems.Any(x => x.Product.Name.Contains(search.ProductName)));
                }
                if (!string.IsNullOrWhiteSpace(search.Sku))
                {
                    query = query.Where(c => c.OrderItems.Any(x => x.Product.Sku.Contains(search.Sku)));
                }
            }

            var result = await query.Include(c => c.Customer)
                .ToStandardTableResult(param, c => new OrderQueryResult
                {
                    Id = c.Id,
                    No = c.No.ToString(),
                    AdminNote = c.AdminNote,
                    BillingAddressId = c.BillingAddressId,
                    CancelOn = c.CancelOn,
                    CancelReason = c.CancelReason,
                    CreatedById = c.CreatedById,
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
                    UpdatedById = c.UpdatedById,
                    UpdatedOn = c.UpdatedOn
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 为指定订单创建发货记录。
        /// </summary>
        /// <param name="id">订单 ID。</param>
        /// <param name="param">订单发货参数。</param>
        /// <returns>创建发货记录操作的结果。</returns>
        [HttpPost("{id:int:min(1)}/shipment")]
        public async Task<Result> Post(int id, [FromBody] OrderShipmentParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var order = await _orderRepository.Query()
                .Include(c => c.OrderItems).ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (order == null)
                return Result.Fail("订单不存在");
            if (order.OrderStatus != OrderStatus.Shipping && order.OrderStatus != OrderStatus.PaymentReceived)
                return Result.Fail($"订单状态处于[{order.OrderStatus}]，不允许发货");

            switch (order.ShippingStatus)
            {
                case ShippingStatus.NoShipping:
                    {
                        //无物流
                        order.OrderStatus = OrderStatus.Shipped;
                        await _orderRepository.SaveChangesAsync();
                        return Result.Ok();
                    }
                case null:
                case ShippingStatus.NotYetShipped:
                case ShippingStatus.PartiallyShipped:
                    order.ShippingStatus = ShippingStatus.PartiallyShipped;
                    break;

                case ShippingStatus.Shipped:
                    return Result.Fail($"订单已发货");

                case ShippingStatus.Delivered:
                    return Result.Fail($"订单已收货");

                default:
                    return Result.Fail($"配送状态异常");
            }
            if (order.OrderStatus == OrderStatus.PaymentReceived)
                order.OrderStatus = OrderStatus.Shipping;

            var shipment = new Shipment
            {
                TrackingNumber = param.TrackingNumber,
                AdminComment = param.AdminComment,
                TotalWeight = param.TotalWeight,
                OrderId = id,
                UpdatedById = currentUser.Id,
                CreatedById = currentUser.Id,
                ShippedOn = DateTime.Now
            };

            foreach (var item in param.Items)
            {
                if (item.QuantityToShip <= 0
                    || shipment.Items.Any(c => c.OrderItemId == item.OrderItemId)
                    || !order.OrderItems.Any(c => c.Id == item.OrderItemId))
                {
                    continue;
                }

                var orderItem = order.OrderItems.First(c => c.Id == item.OrderItemId);
                if (orderItem.ShippedQuantity >= orderItem.Quantity)
                {
                    throw new Exception($"订单商品[{orderItem.Product.Name}]，已全部发货");
                }
                if (orderItem.ShippedQuantity + item.QuantityToShip > orderItem.Quantity)
                {
                    throw new Exception($"订单商品[{orderItem.Product.Name}]，发货数量不能>下单数量");
                }

                var shipmentItem = new ShipmentItem
                {
                    Shipment = shipment,
                    Quantity = item.QuantityToShip,
                    ProductId = orderItem.ProductId,
                    OrderItemId = item.OrderItemId,
                    CreatedById = currentUser.Id,
                    UpdatedById = currentUser.Id
                };
                shipment.Items.Add(shipmentItem);
                orderItem.ShippedQuantity += item.QuantityToShip;
            }

            if (!order.OrderItems.Any(c => c.ShippedQuantity < c.Quantity))
            {
                var timeFromMin = await _appSettingService.Get<int>(OrderKeys.OrderAutoCompleteTimeForMinute);

                //全部发货
                order.ShippingStatus = ShippingStatus.Shipped;
                order.OrderStatus = OrderStatus.Shipped;
                order.ShippedOn = DateTime.Now;
                order.DeliveredEndOn = order.ShippedOn.Value.AddMinutes(timeFromMin);
                order.UpdatedOn = DateTime.Now;
            }

            _shipmentRepository.Add(shipment);

            using (var transaction = _orderRepository.BeginTransaction())
            {
                await _orderRepository.SaveChangesAsync();
                await _shipmentRepository.SaveChangesAsync();
                transaction.Commit();
            }
            return Result.Ok();
        }

        private async Task<OrderAddress> UserAddressToOrderAddress(int userAddressId, int userId, AddressType addressType, Order order)
        {
            var userAddress = await _userAddressRepository.Query()
                    .Include(c => c.Address)
                    .Where(c => c.Id == userAddressId && c.UserId == userId && c.AddressType == addressType)
                    .FirstOrDefaultAsync();
            var shipping = userAddress ?? throw new Exception("配送地址不存在");
            var orderAddress = new OrderAddress()
            {
                Order = order,
                AddressType = shipping.AddressType,
                AddressLine1 = shipping.Address.AddressLine1,
                AddressLine2 = shipping.Address.AddressLine2,
                City = shipping.Address.City,
                Company = shipping.Address.Company,
                ContactName = shipping.Address.ContactName,
                CountryId = shipping.Address.CountryId,
                Email = shipping.Address.Email,
                Phone = shipping.Address.Phone,
                StateOrProvinceId = shipping.Address.StateOrProvinceId,
                ZipCode = shipping.Address.ZipCode
            };
            return orderAddress;
        }

        private async Task<OrderGetResult> GetOrder(int id, long no = 0)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            Order order = null;
            var orderQuery = _orderRepository
                .Query()
                .Include(c => c.Customer)
                .Include(c => c.BillingAddress)
                .Include(c => c.ShippingAddress)
                .Include(c => c.OrderItems);
            if (id <= 0 && no <= 0)
                throw new Exception("参数异常");

            if (id > 0)
                order = await orderQuery.Where(c => c.Id == id).FirstOrDefaultAsync();
            else if (no > 0)
                order = await orderQuery.Where(c => c.No == no).FirstOrDefaultAsync();

            if (order == null)
                throw new Exception("订单不存在");

            var result = new OrderGetResult
            {
                Id = order.Id,
                No = order.No.ToString(),
                AdminNote = order.AdminNote,
                BillingAddressId = order.BillingAddressId,
                CancelOn = order.CancelOn,
                CancelReason = order.CancelReason,
                CreatedById = order.CreatedById,
                CreatedOn = order.CreatedOn,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.FullName,
                DiscountAmount = order.DiscountAmount,
                OrderNote = order.OrderNote,
                OrderStatus = order.OrderStatus,
                OrderTotal = order.OrderTotal,
                PaymentFeeAmount = order.PaymentFeeAmount,
                PaymentMethod = order.PaymentMethod,
                PaymentOn = order.PaymentOn,
                PaymentType = order.PaymentType,
                ShippingAddressId = order.ShippingAddressId,
                ShippingFeeAmount = order.ShippingFeeAmount,
                ShippingMethod = order.ShippingMethod,
                ShippingStatus = order.ShippingStatus,
                UpdatedById = order.UpdatedById,
                UpdatedOn = order.UpdatedOn,
                DeliveredOn = order.DeliveredOn,
                RefundStatus = order.RefundStatus,
                RefundReason = order.RefundReason,
                RefundOn = order.RefundOn,
                RefundAmount = order.RefundAmount,
                ShippedOn = order.ShippedOn
            };
            if (order.BillingAddress != null)
            {
                result.BillingAddress = new OrderGetAddressResult()
                {
                    AddressLine1 = order.BillingAddress.AddressLine1,
                    AddressLine2 = order.BillingAddress.AddressLine2,
                    AddressType = order.BillingAddress.AddressType,
                    City = order.BillingAddress.City,
                    Company = order.BillingAddress.Company,
                    ContactName = order.BillingAddress.ContactName,
                    CountryId = order.BillingAddress.CountryId,
                    Email = order.BillingAddress.Email,
                    Id = order.BillingAddress.Id,
                    Phone = order.BillingAddress.Phone,
                    StateOrProvinceId = order.BillingAddress.StateOrProvinceId,
                    ZipCode = order.BillingAddress.ZipCode
                };
                var provinces = await _countryService.GetProvinceByCache(order.BillingAddress.CountryId);
                IList<string> list = new List<string>();
                _countryService.ProvincesTransformToStringArray(provinces, order.BillingAddress.StateOrProvinceId, ref list);
                result.BillingAddress.StateOrProvinceIds = list;
            }
            if (order.ShippingAddress != null)
            {
                result.ShippingAddress = new OrderGetAddressResult()
                {
                    AddressLine1 = order.ShippingAddress.AddressLine1,
                    AddressLine2 = order.ShippingAddress.AddressLine2,
                    AddressType = order.ShippingAddress.AddressType,
                    City = order.ShippingAddress.City,
                    Company = order.ShippingAddress.Company,
                    ContactName = order.ShippingAddress.ContactName,
                    CountryId = order.ShippingAddress.CountryId,
                    Email = order.ShippingAddress.Email,
                    Id = order.ShippingAddress.Id,
                    Phone = order.ShippingAddress.Phone,
                    StateOrProvinceId = order.ShippingAddress.StateOrProvinceId,
                    ZipCode = order.ShippingAddress.ZipCode
                };
                var provinces = await _countryService.GetProvinceByCache(order.ShippingAddress.CountryId);
                IList<string> list = new List<string>();
                _countryService.ProvincesTransformToStringArray(provinces, order.ShippingAddress.StateOrProvinceId, ref list);
                result.ShippingAddress.StateOrProvinceIds = list;
            }
            foreach (var item in order.OrderItems)
            {
                result.Items.Add(new OrderGetItemResult()
                {
                    Id = item.ProductId,
                    DiscountAmount = item.DiscountAmount,
                    ItemAmount = item.ItemAmount,
                    ItemWeight = item.ItemWeight,
                    MediaUrl = item.ProductMediaUrl,
                    Name = item.ProductName,
                    Note = item.Note,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    ShippedQuantity = item.ShippedQuantity,
                    OrderItemId = item.Id
                });
            }

            return result;
        }

        /// <summary>
        /// 库增加/存减少规则
        /// 当增加下单数量时减少库存
        /// 当减少下单数量时增加库存
        /// </summary>
        /// <param name="stocks"></param>
        /// <param name="product"></param>
        /// <param name="quantity">减少或增加库存数量，减少库存负数，增加库存整数</param>
        /// <param name="orderId"></param>
        /// <param name="note"></param>
        private void OrderStockDoWorker(IList<Stock> stocks, IList<StockHistory> addStockHistories, Product product, User user, int quantity, Order order, string note)
        {
            if (product?.StockTrackingIsEnabled != true || quantity == 0)
                return;

            // 交易取消、交易完成的订单修改下单数量不修改库存
            var notStockOrderStatus = new OrderStatus[] { OrderStatus.Canceled, OrderStatus.Complete };
            if (order == null || notStockOrderStatus.Contains(order.OrderStatus))
                return;

            if (stocks.Count <= 0)
                throw new Exception("商品库存不存在");

            var productStocks = stocks.Where(c => c.ProductId == product.Id && c.IsEnabled);
            if (productStocks.Count() <= 0)
                throw new Exception($"商品：{product.Name}，无可用库存");

            switch (product.StockReduceStrategy)
            {
                case StockReduceStrategy.PlaceOrderWithhold:
                    //下单减库存时，支持成功，不减少库存
                    if (order.OrderStatus == OrderStatus.PaymentReceived)
                        return;
                    break;

                case StockReduceStrategy.PaymentSuccessDeduct:
                    //支付减库存时，下单、待支付、支付失败，不减少库存
                    var oss = new OrderStatus[] { OrderStatus.New, OrderStatus.PendingPayment, OrderStatus.PaymentFailed };
                    if (oss.Contains(order.OrderStatus))
                        return;
                    break;

                default:
                    throw new Exception("库存扣减策略不存在");
            }

            //分布式锁，重新获取库存
            //todo

            if (quantity < 0)
            {
                //减少库存
                var absQuantity = Math.Abs(quantity);
                if (productStocks.Sum(c => c.StockQuantity) < absQuantity)
                    throw new Exception($"商品[{product.Name}]库存不足，库存剩余：{productStocks.Sum(c => c.StockQuantity)}");
                do
                {
                    var firstStock = productStocks.Where(c => c.StockQuantity > 0).OrderBy(c => c.DisplayOrder).FirstOrDefault();
                    if (firstStock == null)
                        throw new Exception($"商品[{product.Name}]库存不足");
                    if (firstStock.StockQuantity >= absQuantity)
                    {
                        firstStock.StockQuantity = firstStock.StockQuantity - absQuantity;
                        if (firstStock.StockQuantity < 0)
                            throw new Exception($"商品[{product.Name}]库存不足");
                        addStockHistories.Add(new StockHistory()
                        {
                            Note = $"订单：{order.No}，商品：{product.Name}，减少库存：{absQuantity}。备注：{note}",
                            CreatedBy = user,
                            UpdatedBy = user,
                            AdjustedQuantity = -absQuantity,
                            StockQuantity = firstStock.StockQuantity,
                            WarehouseId = firstStock.WarehouseId,
                            ProductId = product.Id
                        });
                        absQuantity = 0;
                    }
                    else
                    {
                        absQuantity = absQuantity - firstStock.StockQuantity;
                        if (absQuantity < 0)
                            throw new Exception($"库存扣减异常，请重试");
                        addStockHistories.Add(new StockHistory()
                        {
                            Note = $"订单：{order.No}，商品：{product.Name}，减少库存：{absQuantity}。备注：{note}",
                            CreatedBy = user,
                            UpdatedBy = user,
                            AdjustedQuantity = -firstStock.StockQuantity,
                            StockQuantity = 0,
                            WarehouseId = firstStock.WarehouseId,
                            ProductId = product.Id
                        });
                        firstStock.StockQuantity = 0;
                    }
                } while (absQuantity > 0);
            }
            else if (quantity > 0)
            {
                //增加库存
                var firstStock = productStocks.OrderBy(c => c.DisplayOrder).FirstOrDefault();
                if (firstStock == null)
                    throw new Exception($"商品：{product.Name}，无可用库存");
                firstStock.StockQuantity += quantity;

                addStockHistories.Add(new StockHistory()
                {
                    Note = $"订单：{order.No}，商品：{product.Name}，增加库存（减少下单商品数量）：{quantity}。备注：{note}",
                    CreatedBy = user,
                    UpdatedBy = user,
                    AdjustedQuantity = quantity,
                    StockQuantity = firstStock.StockQuantity,
                    WarehouseId = firstStock.WarehouseId,
                    ProductId = product.Id
                });
            }
        }
    }
}