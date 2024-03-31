using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.ShoppingCart.Entities;
using Shop.Module.ShoppingCart.Services;
using Shop.Module.ShoppingCart.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.ShoppingCart.Controllers
{
    /// <summary>
    /// 购物车 API 控制器，负责处理购物车相关操作。
    /// </summary>
    [Authorize()]
    [Route("api/cart")]
    public class CartApiController : ControllerBase
    {
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly ICartService _cartService;
        private readonly IWorkContext _workContext;

        public CartApiController(
            IRepository<CartItem> cartItemRepository,
            ICartService cartService,
            IWorkContext workContext)
        {
            _cartItemRepository = cartItemRepository;
            _cartService = cartService;
            _workContext = workContext;
        }

        /// <summary>
        /// 获取当前用户的购物车列表，未登录用户返回空购物车。
        /// </summary>
        /// <returns>购物车内容。</returns>
        [HttpGet()]
        [AllowAnonymous]
        public async Task<Result> List()
        {
            var user = await _workContext.GetCurrentUserOrNullAsync();
            var cart = new CartResult();
            if (user != null)
            {
                cart = await _cartService.GetActiveCartDetails(user.Id);
            }
            return Result.Ok(cart);
        }

        /// <summary>
        /// 向购物车中添加商品。
        /// </summary>
        /// <param name="model">添加到购物车的商品信息。</param>
        /// <returns>更新后的购物车内容。</returns>
        [HttpPost("add-item")]
        public async Task<Result> AddToCart([FromBody]AddToCartParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            await _cartService.AddToCart(currentUser.Id, model.ProductId, model.Quantity);
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            return Result.Ok(cart);
        }

        /// <summary>
        /// 更新购物车中某个商品的数量。
        /// </summary>
        /// <param name="model">商品和新数量的信息。</param>
        /// <returns>更新后的购物车内容。</returns>
        [HttpPut("update-item-quantity")]
        public async Task<Result> UpdateItemQuantity([FromBody]AddToCartParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            await _cartService.UpdateItemQuantity(currentUser.Id, currentUser.Id, model.ProductId, model.Quantity);
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            return Result.Ok(cart);
        }

        /// <summary>
        /// 选中或取消选中购物车中的商品项。
        /// </summary>
        /// <param name="model">商品项选中状态的信息。</param>
        /// <returns>更新后的购物车内容。</returns>
        [HttpPut("checked")]
        public async Task<Result> CheckedItem([FromBody]CheckedItemParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            await _cartService.CheckedItem(currentUser.Id, model);
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            return Result.Ok(cart);
        }

        /// <summary>
        /// 从购物车中移除一个或多个商品。
        /// </summary>
        /// <param name="model">要移除的商品的ID列表。</param>
        /// <returns>更新后的购物车内容。</returns>
        [HttpDelete("remove-items")]
        public async Task<Result> Remove([FromBody]DeleteItemParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var cartItems = _cartItemRepository.Query().Where(x => model.ProductIds.Contains(x.ProductId) && x.Cart.CustomerId == currentUser.Id).ToList();
            foreach (var item in cartItems)
            {
                item.IsDeleted = true;
                item.UpdatedOn = DateTime.Now;
                _cartItemRepository.SaveChanges();
            }
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            return Result.Ok(cart);
        }

        //[HttpGet("api/customers/{customerId}/cart")]
        //public async Task<IActionResult> List(long customerId)
        //{
        //    var currentUser = await _workContext.GetCurrentUser();
        //    var cart = await _cartService.GetActiveCartDetails(customerId, currentUser.Id);

        //    return Json(cart);
        //}

        //[HttpPost("api/customers/{customerId}/add-cart-item")]
        //public async Task<IActionResult> AddToCart(long customerId, [FromBody] AddToCartModel model)
        //{
        //    var currentUser = await _workContext.GetCurrentUser();
        //    await _cartService.AddToCart(customerId, currentUser.Id, model.ProductId, model.Quantity);

        //    return Accepted();
        //}

        //[HttpPut("api/carts/items/{itemId}")]
        //public async Task<IActionResult> UpdateQuantity(long itemId, [FromBody] CartQuantityUpdate model)
        //{
        //    var currentUser = await _workContext.GetCurrentUser();
        //    var cartItem = _cartItemRepository.Query().FirstOrDefault(x => x.Id == itemId && x.Cart.CreatedById == currentUser.Id);
        //    if (cartItem == null)
        //    {
        //        return NotFound();
        //    }

        //    cartItem.Quantity = model.Quantity;
        //    _cartItemRepository.SaveChanges();

        //    return Accepted();
        //}

        //[HttpPost("api/carts/{cartId}/apply-coupon")]
        //public async Task<ActionResult> ApplyCoupon(long cartId, [FromBody] ApplyCouponForm model)
        //{
        //    var currentUser = await _workContext.GetCurrentUser();
        //    var cart = await _cartService.Query().FirstOrDefaultAsync(x => x.Id == cartId && x.CreatedById == currentUser.Id);
        //    if (cart == null)
        //    {
        //        return NotFound();
        //    }

        //    var validationResult = await _cartService.ApplyCoupon(cart.Id, model.CouponCode);
        //    if (validationResult.Succeeded)
        //    {
        //        var cartVm = await _cartService.GetActiveCartDetails(currentUser.Id);
        //        return Json(cartVm);
        //    }

        //    return Json(validationResult);
        //}

        //[HttpPost("api/carts/{cartId}/save-ordernote")]
        //public async Task<IActionResult> SaveOrderNote(long cartId, [FromBody] SaveOrderNote model)
        //{
        //    var currentUser = await _workContext.GetCurrentUser();
        //    var cart = await _cartService.Query().FirstOrDefaultAsync(x => x.Id == cartId && x.CreatedById == currentUser.Id);
        //    if (cart == null)
        //    {
        //        return NotFound();
        //    }

        //    cart.OrderNote = model.OrderNote;
        //    await _cartItemRepository.SaveChangesAsync();
        //    return Accepted();
        //}

        //[HttpDelete("api/carts/items/{itemId}")]
        //public async Task<IActionResult> Remove(long itemId)
        //{
        //    var currentUser = await _workContext.GetCurrentUser();
        //    var cartItem = _cartItemRepository.Query().FirstOrDefault(x => x.Id == itemId && x.Cart.CreatedById == currentUser.Id);
        //    if (cartItem == null)
        //    {
        //        return NotFound();
        //    }

        //    _cartItemRepository.Remove(cartItem);
        //    _cartItemRepository.SaveChanges();

        //    return NoContent();
        //}
    }
}
