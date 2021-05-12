using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Services;
using Shop.Module.ShoppingCart.Entities;
using Shop.Module.ShoppingCart.Services;
using Shop.Module.ShoppingCart.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.ShoppingCart.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IMediaService _mediaService;
        //private readonly ICouponService _couponService;

        public CartService(
            IRepository<Cart> cartRepository,
            IRepository<CartItem> cartItemRepository,
            //ICouponService couponService,
            IMediaService mediaService,
            IConfiguration config)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            //_couponService = couponService;
            _mediaService = mediaService;
        }

        public IQueryable<Cart> Query()
        {
            return _cartRepository.Query();
        }

        public IQueryable<Cart> GetActiveCart(int customerId)
        {
            return GetActiveCart(customerId, customerId);
        }

        public IQueryable<Cart> GetActiveCart(int customerId, int createdById)
        {
            return _cartRepository.Query()
                .Include(x => x.Items)
                .Where(x => x.CustomerId == customerId && x.CreatedById == createdById && x.IsActive);
        }

        public async Task AddToCart(int customerId, int productId, int quantity)
        {
            await AddToCart(customerId, customerId, productId, quantity);
        }

        public async Task AddToCart(int customerId, int createdById, int productId, int quantity)
        {
            if (quantity < 0)
                quantity = 0;
            else if (quantity > 999)
                quantity = 999;

            var cart = await GetActiveCart(customerId, createdById).Include(x => x.Items).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedById = createdById,
                    UpdatedById = createdById
                };
                _cartRepository.Add(cart);
            }
            var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (cartItem == null)
            {
                if (quantity <= 0)
                    return;
                cartItem = new CartItem
                {
                    Cart = cart,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedById = createdById,
                    UpdatedById = createdById
                };
                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;

                if (cartItem.Quantity > 999)
                    cartItem.Quantity = 999;

                cartItem.UpdatedOn = DateTime.Now;
                cartItem.UpdatedById = createdById;
            }
            await _cartRepository.SaveChangesAsync();
        }

        public async Task UpdateItemQuantity(int customerId, int createdById, int productId, int quantity)
        {
            if (quantity < 0)
                quantity = 0;
            else if (quantity > 999)
                quantity = 999;

            var cartItem = _cartItemRepository
                .Query()
                .Include(c => c.Cart)
                .FirstOrDefault(x => x.ProductId == productId && x.Cart.CustomerId == customerId);
            if (cartItem == null)
            {
                if (quantity <= 0)
                    return;

                var cart = await GetActiveCart(customerId).FirstOrDefaultAsync();
                if (cart == null)
                {
                    cart = new Cart
                    {
                        CustomerId = customerId,
                        CreatedById = createdById,
                        UpdatedById = createdById
                    };
                    _cartRepository.Add(cart);
                }
                cartItem = new CartItem
                {
                    Cart = cart,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedById = createdById,
                    UpdatedById = createdById
                };
                cart.Items.Add(cartItem);
                await _cartRepository.SaveChangesAsync();
            }
            else
            {
                if (quantity <= 0)
                {
                    cartItem.IsDeleted = true;
                }
                cartItem.Quantity = quantity;
                cartItem.UpdatedOn = DateTime.Now;
                cartItem.UpdatedById = createdById;
                _cartItemRepository.SaveChanges();
            }
        }

        public async Task CheckedItem(int customerId, CheckedItemParam model)
        {
            var items = await _cartItemRepository.Query()
               .Include(c => c.Cart)
               .Where(x => model.ProductIds.Contains(x.ProductId) && x.Cart.CustomerId == customerId && x.IsChecked == !model.IsChecked)
               .ToListAsync();

            foreach (var item in items)
            {
                item.IsChecked = model.IsChecked;
                item.UpdatedOn = DateTime.Now;
                item.UpdatedById = customerId;
            }
            _cartItemRepository.SaveChanges();
        }

        public async Task<CartResult> GetActiveCartDetails(int customerId)
        {
            return await GetActiveCartDetails(customerId, customerId);
        }

        // TODO separate getting product thumbnail, varation options from here
        public async Task<CartResult> GetActiveCartDetails(int customerId, int createdById)
        {
            var cart = await GetActiveCart(customerId, createdById).FirstOrDefaultAsync();
            if (cart == null)
            {
                return new CartResult();
            }

            var cartVm = new CartResult()
            {
                Id = cart.Id,
                CouponCode = cart.CouponCode,
                ShippingAmount = cart.ShippingAmount,
                OrderNote = cart.OrderNote
            };

            cartVm.Items = _cartItemRepository.Query()
                .Include(x => x.Product).ThenInclude(p => p.ThumbnailImage)
                .Include(x => x.Product).ThenInclude(p => p.OptionCombinations).ThenInclude(o => o.Option)
                .Where(x => x.CartId == cart.Id)
                .Select(x => new CartItemResult
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    ProductPrice = x.Product.Price,
                    IsChecked = x.IsChecked,
                    Quantity = x.Quantity,
                    ProductImage = x.Product.ThumbnailImage.Url,
                    VariationOptions = x.Product.OptionCombinations.Select(c => new ProductVariationOption
                    {
                        OptionName = c.Option.Name,
                        Value = c.Value
                    })
                }).ToList();

            //if (!string.IsNullOrWhiteSpace(cartVm.CouponCode))
            //{
            //    var cartInfoForCoupon = new CartInfoForCoupon
            //    {
            //        Items = cartVm.Items.Select(x => new CartItemForCoupon { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()
            //    };
            //    var couponValidationResult = await _couponService.Validate(customerId, cartVm.CouponCode, cartInfoForCoupon);
            //    if (couponValidationResult.Succeeded)
            //    {
            //        cartVm.Discount = couponValidationResult.DiscountAmount;
            //    }
            //    else
            //    {
            //        cartVm.CouponValidationErrorMessage = couponValidationResult.ErrorMessage;
            //    }
            //}

            return cartVm;
        }

        //public async Task<CouponValidationResult> ApplyCoupon(int cartId, string couponCode)
        //{
        //    var cart = _cartRepository.Query().Include(x => x.Items).FirstOrDefault(x => x.Id == cartId);

        //    var cartInfoForCoupon = new CartInfoForCoupon
        //    {
        //        Items = cart.Items.Select(x => new CartItemForCoupon { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()
        //    };
        //    var couponValidationResult = await _couponService.Validate(cart.CustomerId, couponCode, cartInfoForCoupon);
        //    if (couponValidationResult.Succeeded)
        //    {
        //        cart.CouponCode = couponCode;
        //        cart.CouponRuleName = couponValidationResult.CouponRuleName;
        //        _cartItemRepository.SaveChanges();
        //    }

        //    return couponValidationResult;
        //}

        public async Task MigrateCart(int fromUserId, int toUserId)
        {
            var cartFrom = GetActiveCart(fromUserId).FirstOrDefault();
            if (cartFrom != null && cartFrom.Items.Any())
            {
                var cartTo = GetActiveCart(toUserId).FirstOrDefault();
                if (cartTo == null)
                {
                    cartTo = new Cart
                    {
                        CustomerId = toUserId,
                        CreatedById = toUserId,
                        UpdatedById = toUserId,
                    };
                    _cartRepository.Add(cartTo);
                }

                foreach (var fromItem in cartFrom.Items)
                {
                    var toItem = cartTo.Items.FirstOrDefault(x => x.ProductId == fromItem.ProductId);
                    if (toItem == null)
                    {
                        toItem = new CartItem
                        {
                            Cart = cartTo,
                            ProductId = fromItem.ProductId,
                            Quantity = fromItem.Quantity,
                            CreatedById = toUserId,
                            UpdatedById = toUserId,
                        };
                        cartTo.Items.Add(toItem);
                    }
                    else
                    {
                        toItem.Quantity = toItem.Quantity + fromItem.Quantity;
                        toItem.UpdatedOn = DateTime.Now;
                        toItem.UpdatedById = toUserId;
                    }
                }

                await _cartRepository.SaveChangesAsync();
            }
        }

        // 暂不控制，下单时控制
        //async Task ValidateAddToCard()
        //{

        //}
    }
}
