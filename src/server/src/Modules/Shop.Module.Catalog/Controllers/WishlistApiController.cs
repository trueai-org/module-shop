using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Extensions;

namespace Shop.Module.Catalog.Controllers
{
    /// <summary>
    /// 控制器用于处理心愿单相关操作的 API 请求。
    /// </summary>
    [Authorize()]
    [Route("api/wishlist")]
    public class WishlistApiController : ControllerBase
    {
        private readonly IRepository<ProductWishlist> _productWishlistRepository;
        private readonly IWorkContext _workContext;

        public WishlistApiController(
            IRepository<ProductWishlist> productWishlistRepository,
            IWorkContext workContext)
        {
            _productWishlistRepository = productWishlistRepository;
            _workContext = workContext;
        }

        /// <summary>
        /// 根据产品 ID 获取心愿单收藏状态。
        /// </summary>
        /// <param name="productId">产品 ID。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpGet("collect-status/{productId:int:min(1)}")]
        [AllowAnonymous]
        public async Task<Result> CollectStatus(int productId)
        {
            var user = await _workContext.GetCurrentUserOrNullAsync();
            if (user != null)
            {
                var any = await _productWishlistRepository.Query()
                    .AnyAsync(c => c.CustomerId == user.Id && c.ProductId == productId);
                return Result.Ok(any);
            }
            return Result.Ok(false);
        }

        /// <summary>
        /// 获取当前用户的心愿单列表。
        /// </summary>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpGet()]
        public async Task<Result> List()
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var list = await _productWishlistRepository.Query()
                .Include(c => c.Product).ThenInclude(x => x.ThumbnailImage)
                .Where(c => c.CustomerId == user.Id)
                .OrderByDescending(c => c.UpdatedOn)
                .Select(c => new GoodsListResult
                {
                    Id = c.Product.Id,
                    Name = c.Product.Name,
                    Slug = c.Product.Slug,
                    Price = c.Product.Price,
                    OldPrice = c.Product.OldPrice,
                    SpecialPrice = c.Product.SpecialPrice,
                    SpecialPriceStart = c.Product.SpecialPriceStart,
                    SpecialPriceEnd = c.Product.SpecialPriceEnd,
                    IsAllowToOrder = c.Product.IsAllowToOrder,
                    ThumbnailUrl = c.Product.ThumbnailImage.Url,
                    ReviewsCount = c.Product.ReviewsCount,
                    RatingAverage = c.Product.RatingAverage,
                    ShortDescription = c.Product.ShortDescription,
                    IsPublished = c.Product.IsPublished,
                    IsFeatured = c.Product.IsFeatured
                }).ToListAsync();
            return Result.Ok(list);
        }

        /// <summary>
        /// 将指定产品添加到心愿单。
        /// </summary>
        /// <param name="param">要添加到心愿单的参数。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpPost()]
        public async Task<Result> Add([FromBody] WishlistAddParam param)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _productWishlistRepository
                .Query().FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.ProductId == param.ProductId);
            if (model == null)
            {
                model = new ProductWishlist()
                {
                    CustomerId = user.Id,
                    ProductId = param.ProductId
                };
                _productWishlistRepository.Add(model);
            }
            model.UpdatedOn = DateTime.Now;
            await _productWishlistRepository.SaveChangesAsync();
            return Result.Ok();
        }

        /// <summary>
        /// 根据产品 ID 从心愿单中移除产品。
        /// </summary>
        /// <param name="productId">产品 ID。</param>
        /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
        [HttpDelete("{productId:int:min(1)}")]
        public async Task<Result> Delete(int productId)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _productWishlistRepository.Query().FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.ProductId == productId);
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedOn = DateTime.Now;
                await _productWishlistRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }
    }
}