using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
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

        [HttpPost()]
        public async Task<Result> Add([FromBody]WishlistAddParam param)
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
