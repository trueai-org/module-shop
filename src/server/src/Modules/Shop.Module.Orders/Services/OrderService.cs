using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Models;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using Shop.Module.Inventory.Entities;
using Shop.Module.Orders.Data;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Models;
using Shop.Module.Orders.ViewModels;
using Shop.Module.Payments.Models;
using Shop.Module.Payments.Services;
using Shop.Module.Schedule;
using Shop.Module.Shipments.Entities;
using Shop.Module.ShoppingCart.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
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
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IUserAddressService _userAddressService;
        private readonly IJobService _jobService;
        private readonly IAppSettingService _appSettingService;
        private readonly ILocker _locker;
        private readonly IPaymentService _paymentService;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly ShopOptions _shopConfig;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
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
            IRepository<Cart> cartRepository,
            IRepository<CartItem> cartItemRepository,
            IUserAddressService userAddressService,
            IJobService jobService,
            IAppSettingService appSettingService,
            ILocker locker,
            IPaymentService paymentService,
            IRepository<UserLogin> userLoginRepository,
            IOptionsSnapshot<ShopOptions> shopConfig)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
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
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _userAddressService = userAddressService;
            _jobService = jobService;
            _appSettingService = appSettingService;
            _locker = locker;
            _paymentService = paymentService;
            _userLoginRepository = userLoginRepository;
            _shopConfig = shopConfig.Value;
        }

        public async Task<OrderCreateResult> OrderCreateByCart(int cartId, OrderCreateBaseParam param, string adminNote = null)
        {
            if (param == null)
                throw new Exception("参数异常");

            var user = await _workContext.GetCurrentUserAsync();
            var customerId = param.CustomerId;

            return await OrderCrateByCart2(cartId, param, adminNote);

            // TODO 下单锁
            //var isLocker = _locker.PerformActionWithLock(OrderKeys.CustomerCreateOrderLock + customerId, TimeSpan.FromSeconds(10),
            //    async () => await OrderCrateByCart2(cartId, param, adminNote));

            //if (!isLocker)
            //{
            //    throw new Exception("正在下单,请勿重复操作");
            //}
        }


        public async Task<OrderCreateResult> OrderCrateByCart2(int cartId, OrderCreateBaseParam param, string adminNote = null)
        {
            if (param == null)
                throw new Exception("参数异常");

            var user = await _workContext.GetCurrentOrThrowAsync();
            var customerId = param.CustomerId;
            var cart = await _cartRepository.Query()
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsActive && x.Id == cartId);

            if (cart == null || cart.Items == null || cart.Items.Where(c => c.IsChecked).Count() <= 0)
            {
                throw new Exception("请选择商品");
            }
            if (cart.Items.Where(c => c.IsChecked).Any(c => c.Quantity <= 0))
            {
                throw new Exception("商品数量必须大于0");
            }
            param.Items = cart.Items.Where(c => c.IsChecked).Select(c => new OrderCreateBaseItemParam()
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity
            }).ToList();

            var order = await this.OrderCreate(user.Id, param, adminNote);

            // 清除已下单的商品
            foreach (var item in cart.Items.Where(c => c.IsChecked))
            {
                if (order.OrderItems.Any(c => c.ProductId == item.ProductId))
                {
                    item.IsDeleted = true;
                    item.UpdatedOn = DateTime.Now;
                }
            }

            await _cartRepository.SaveChangesAsync();

            return new OrderCreateResult()
            {
                OrderId = order.Id,
                OrderNo = order.No.ToString(),
                OrderTotal = order.OrderTotal
            };
        }

        public async Task<Order> OrderCreate(int userId, OrderCreateBaseParam param, string adminNote = null)
        {
            if (param == null)
                throw new Exception("参数异常");
            if (param.Items == null || param.Items.Count <= 0)
                throw new Exception("购买商品不存在或已下架");
            if (param.Items.Any(c => c.Quantity <= 0))
                throw new Exception("购买商品数量必须>0");

            param.Items = param.Items.GroupBy(c => c.ProductId).Select(c => new OrderCreateBaseItemParam()
            {
                ProductId = c.Key,
                Quantity = c.Sum(x => x.Quantity)
            }).ToList();

            var customerId = param.CustomerId;
            var payEndTimeForMin = await _appSettingService.Get<int>(OrderKeys.OrderAutoCanceledTimeForMinute);
            var productIds = param.Items.Select(c => c.ProductId).Distinct();
            var products = await _productRepository.Query()
                .Where(c => productIds.Contains(c.Id) && c.IsPublished && c.IsAllowToOrder)
                .Include(c => c.ThumbnailImage)
                .ToListAsync();
            if (products == null || products.Count <= 0)
                throw new Exception("购买商品不存在或已下架");

            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var order = new Order()
            {
                OrderStatus = OrderStatus.PendingPayment, // 默认创建订单，待付款
                CreatedById = userId,
                UpdatedById = userId,
                CustomerId = customerId,
                AdminNote = adminNote,
                OrderNote = param.OrderNote,
                ShippingMethod = param.ShippingMethod,
                PaymentType = param.PaymentType,
                ShippingFeeAmount = param.ShippingFeeAmount,
                DiscountAmount = param.DiscountAmount,
                PaymentEndOn = DateTime.Now.AddMinutes(payEndTimeForMin)
            };

            var orderShipping = await UserAddressToOrderAddress(param.ShippingUserAddressId, customerId, AddressType.Shipping, order);
            OrderAddress orderBilling = null;
            if (param.BillingUserAddressId.HasValue && param.BillingUserAddressId.Value > 0)
            {
                orderBilling = await this.UserAddressToOrderAddress(param.BillingUserAddressId.Value, customerId, AddressType.Billing, order);
            }

            var addStockHistories = new List<StockHistory>();
            foreach (var item in param.Items)
            {
                var product = products.FirstOrDefault(c => c.Id == item.ProductId);
                if (product == null)
                    continue;

                OrderStockDoWorker(stocks, addStockHistories, product, userId, -item.Quantity, order, "创建订单");

                var orderItem = new OrderItem()
                {
                    Order = order,
                    Product = product,
                    ItemWeight = 0,
                    ItemAmount = item.Quantity * product.Price,
                    Quantity = item.Quantity,
                    ProductPrice = product.Price,
                    DiscountAmount = 0,
                    CreatedById = userId,
                    UpdatedById = userId,
                    ProductName = product.Name,
                    ProductMediaUrl = product.ThumbnailImage?.Url
                };
                order.OrderItems.Add(orderItem);
            }

            order.SubTotal = order.OrderItems.Sum(c => c.Quantity * c.ProductPrice);
            order.SubTotalWithDiscount = order.OrderItems.Sum(c => c.DiscountAmount);
            order.OrderTotal = order.OrderItems.Sum(c => c.Product.Price * c.Quantity) + order.ShippingFeeAmount - order.SubTotalWithDiscount - order.DiscountAmount;
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

                // 订单取消任务 如果n分钟内用户支付或主动取消订单时,则自动取消此任务
                var min = await _appSettingService.Get<int>(OrderKeys.OrderAutoCanceledTimeForMinute);
                await _jobService.Schedule(() => Cancel(order.Id, (int)UserWithId.System, "超时自动取消"), TimeSpan.FromMinutes(min));

                transaction.Commit();
            }

            //TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
            //using (var ts = new TransactionScope())
            //{
            //    ts.Complete();
            //}

            return order;
        }

        public async Task Cancel(int id, int userId, string reason)
        {
            //var currentUser = await _workContext.GetCurrentUser();
            var order = await _orderRepository.Query()
                .Include(c => c.OrderItems).ThenInclude(c => c.Product)
                .Where(c => c.Id == id).FirstOrDefaultAsync();

            if (order == null || order.OrderStatus == OrderStatus.Canceled)
            {
                return;
            }
            var orderSs = new OrderStatus[] { OrderStatus.New, OrderStatus.PendingPayment, OrderStatus.PaymentFailed };
            if (!orderSs.Contains(order.OrderStatus))
            {
                return;
            }

            var productIds = order.OrderItems.Select(c => c.ProductId).Distinct();
            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var addStockHistories = new List<StockHistory>();
            //取消订单增加库存
            foreach (var item in order.OrderItems)
            {
                OrderStockDoWorker(stocks, addStockHistories, item.Product, userId, item.Quantity, order, "取消订单");
            }
            var oldStatus = order.OrderStatus;
            order.OrderStatus = OrderStatus.Canceled;
            order.CancelReason = reason;
            order.UpdatedOn = DateTime.Now;
            order.UpdatedById = userId;

            using (var transaction = _orderRepository.BeginTransaction())
            {
                await _orderRepository.SaveChangesAsync();
                var orderStatusChanged = new OrderChanged
                {
                    OrderId = order.Id,
                    OldStatus = oldStatus,
                    NewStatus = order.OrderStatus,
                    Order = order,
                    UserId = userId,
                    Note = "取消订单"
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

            //var orderItems = _orderItemRepository.Query().Include(x => x.Product).Where(x => x.Order.Id == order.Id);
            //foreach (var item in orderItems)
            //{
            //    //if (item.Product.StockTrackingIsEnabled)
            //    //{
            //    //    item.Product.StockQuantity = item.Product.StockQuantity + item.Quantity;
            //    //}
            //    // 库存处理
            //}
        }

        /// <summary>
        /// 获取订单预支付信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<PaymentOrderBaseResponse> PayInfo(int orderId)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var order = await _orderRepository.Query().FirstOrDefaultAsync(c => c.Id == orderId && c.CustomerId == user.Id);
            if (order == null)
            {
                throw new Exception("订单不存在");
            }
            else if (order.OrderStatus != OrderStatus.New && order.OrderStatus != OrderStatus.PendingPayment && order.OrderStatus != OrderStatus.PaymentFailed)
            {
                throw new Exception("当前订单状态不允许支付");
            }

            var userLogin = await _userLoginRepository.Query().FirstOrDefaultAsync(c => c.UserId == user.Id); // && c.LoginProvider ==
            if (string.IsNullOrWhiteSpace(userLogin?.ProviderKey))
            {
                throw new Exception("买家信息信息异常");
            }

            var shopConfig = _shopConfig;
            if (string.IsNullOrWhiteSpace(shopConfig?.ShopName))
            {
                throw new ArgumentNullException(nameof(ShopOptions.ShopName));
            }

            // 生成第三方预支付订单信息
            var result = await _paymentService.GeneratePaymentOrder(new PaymentOrderRequest()
            {
                OrderNo = order.No.ToString(),
                OrderId = order.Id,
                OpenId = userLogin.ProviderKey,
                Subject = shopConfig.ShopName,
                TotalAmount = order.OrderTotal
            });
            return result;
        }

        async Task<OrderAddress> UserAddressToOrderAddress(int userAddressId, int userId, AddressType addressType, Order order)
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
        void OrderStockDoWorker(IList<Stock> stocks, IList<StockHistory> addStockHistories, Product product, int userId, int quantity, Order order, string note)
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
                            CreatedById = userId,
                            UpdatedById = userId,
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
                            CreatedById = userId,
                            UpdatedById = userId,
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
                    CreatedById = userId,
                    UpdatedById = userId,
                    AdjustedQuantity = quantity,
                    StockQuantity = firstStock.StockQuantity,
                    WarehouseId = firstStock.WarehouseId,
                    ProductId = product.Id
                });
            }
        }

        public async Task<OrderAddressResult> GetOrderAddress(int orderAddressId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var countryId = (int)CountryWithId.China;
            var provinces = await _countryService.GetProvinceByCache(countryId);
            if (provinces == null || provinces.Count <= 0)
                throw new Exception("省市区数据异常，请联系管理员");

            var query = _orderAddressRepository
                .Query()
                .Where(c => c.Id == orderAddressId)
                .Include(c => c.Country)
                .Where(x => x.AddressType == AddressType.Shipping);

            var model = await query.Select(x => new OrderAddressResult
            {
                Id = x.Id,
                ContactName = x.ContactName,
                Phone = x.Phone,
                AddressLine1 = x.AddressLine1,
                ZipCode = x.ZipCode,
                CountryId = x.CountryId,
                CountryName = x.Country.Name,
                StateOrProvinceId = x.StateOrProvinceId,

            }).FirstOrDefaultAsync();

            if (model != null)
            {
                var first = provinces.FirstOrDefault(c => c.Id == model.StateOrProvinceId);
                if (first != null)
                {
                    StateOrProvinceDto pro = null, city = null, district = null;
                    if (first.Level == StateOrProvinceLevel.Default)
                    {
                        pro = first;
                    }
                    else if (first.Level == StateOrProvinceLevel.City)
                    {
                        city = first;
                        pro = provinces.FirstOrDefault(c => c.Id == city?.ParentId);
                    }
                    else if (first.Level == StateOrProvinceLevel.District)
                    {
                        district = first;
                        city = provinces.FirstOrDefault(c => c.Id == district?.ParentId);
                        pro = provinces.FirstOrDefault(c => c.Id == city?.ParentId);
                    }
                    model.StateOrProvinceId = pro?.Id ?? 0;
                    model.StateOrProvinceName = pro?.Name;
                    model.CityId = city?.Id ?? 0;
                    model.CityName = city?.Name;
                    model.DistrictId = district?.Id;
                    model.DistrictName = district?.Name;
                }
            }
            return model;
        }

        public async Task PaymentReceived(PaymentReceivedParam param)
        {
            if (param == null)
                return;

            var orderSs = new OrderStatus[] { OrderStatus.New, OrderStatus.PendingPayment, OrderStatus.PaymentFailed };
            var user = await _workContext.GetCurrentUserOrNullAsync();
            var userId = user?.Id ?? (int)UserWithId.System;
            var query = _orderRepository.Query();
            if (param.OrderId.HasValue && param.OrderId.Value > 0)
            {
                query = query.Where(c => c.Id == param.OrderId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(param.OrderNo))
            {
                var longOrderNo = Convert.ToInt64(param.OrderNo);
                query = query.Where(c => c.No == longOrderNo);
            }
            else
            {
                return;
            }

            var order = await query.Include(c => c.OrderItems).ThenInclude(c => c.Product)
                .FirstOrDefaultAsync();

            if (order == null || !orderSs.Contains(order.OrderStatus))
            {
                return;
            }

            order.OrderStatus = OrderStatus.PaymentReceived;
            order.PaymentMethod = param.PaymentMethod;
            order.PaymentOn = param.PaymentOn ?? DateTime.Now;
            order.PaymentFeeAmount = param.PaymentFeeAmount ?? order.OrderTotal;
            order.UpdatedOn = DateTime.Now;
            order.UpdatedById = userId;

            //标记付款，付款减少库存商品，减少库存
            var productIds = order.OrderItems.Select(c => c.ProductId).Distinct();
            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var addStockHistories = new List<StockHistory>();
            foreach (var item in order.OrderItems)
            {
                OrderStockDoWorker(stocks, addStockHistories, item.Product, userId, -item.Quantity, order, "标记付款");
            }

            using (var transaction = _orderRepository.BeginTransaction())
            {
                await _orderRepository.SaveChangesAsync();
                await _stockRepository.SaveChangesAsync();

                if (addStockHistories.Count > 0)
                {
                    _stockHistoryRepository.AddRange(addStockHistories);
                    await _stockHistoryRepository.SaveChangesAsync();
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// 验证并获取预下单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<CheckoutResult> OrderCheckout(CheckoutParam param)
        {
            if (param == null)
                throw new Exception("参数异常");
            if (param.Items == null || param.Items.Count <= 0)
                throw new Exception("购买商品不存在或已下架");
            if (param.Items.Any(c => c.Quantity <= 0))
                throw new Exception("购买商品数量必须>0");

            var user = await _workContext.GetCurrentUserOrNullAsync();
            var userId = user?.Id ?? (int)UserWithId.System;
            var customerId = param.CustomerId;
            var customer = await _userRepository.FirstOrDefaultAsync(customerId);
            if (customer == null)
                throw new Exception("客户不存在");

            var ids = param.Items.Select(c => c.ProductId).Distinct();
            var products = await _productRepository.Query()
                .Where(c => ids.Contains(c.Id))
                .Include(p => p.ThumbnailImage)
                .Include(p => p.OptionCombinations).ThenInclude(o => o.Option)
                .Select(x => new CheckoutItemResult
                {
                    Id = x.Id,
                    ProductId = x.Id,
                    //Quantity = x.Quantity,
                    ProductName = x.Name,
                    ProductPrice = x.Price,
                    IsAllowToOrder = x.IsAllowToOrder,
                    DisplayStockAvailability = x.DisplayStockAvailability,
                    DisplayStockQuantity = x.DisplayStockQuantity,
                    StockTrackingIsEnabled = x.StockTrackingIsEnabled,
                    ProductImage = x.ThumbnailImage.Url,
                    IsPublished = x.IsPublished,
                    VariationOptions = x.OptionCombinations.Select(c => new ProductVariationOption
                    {
                        OptionName = c.Option.Name,
                        Value = c.Value
                    })
                }).ToListAsync();

            if (products == null || products.Count <= 0)
            {
                throw new Exception("购买商品不存在或已下架");
            }

            var productIds = products.Select(c => c.ProductId).Distinct();
            var stocks = await _stockRepository.Query().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            products.ForEach(c =>
            {
                c.Quantity = param.Items.First(x => x.ProductId == c.ProductId).Quantity;
                c.StockQuantity = stocks.Where(x => x.ProductId == c.ProductId && x.IsEnabled).Sum(x => x.StockQuantity);
                if (!c.IsPublished)
                {
                    c.IsAllowToOrder = false;
                    c.StockQuantity = 0;
                }
                if (c.IsAllowToOrder && c.StockTrackingIsEnabled)
                {
                    if (c.StockQuantity <= 0)
                    {
                        c.IsAllowToOrder = false;
                    }
                    else
                    {
                        if (!c.DisplayStockQuantity)
                        {
                            c.StockQuantity = 0; // 不显示库存量
                        }
                    }
                }
            });

            var userAddressId = param.UserAddressId ?? customer.DefaultBillingAddressId;
            UserAddressShippingResult address = null;
            if (userAddressId.HasValue)
            {
                var list = await _userAddressService.GetList(userAddressId.Value);
                address = list.FirstOrDefault();
            }

            return new CheckoutResult()
            {
                CouponCode = "",
                ShippingAmount = 0,
                Discount = 0,
                Address = address,
                Items = products
            };
        }
    }
}