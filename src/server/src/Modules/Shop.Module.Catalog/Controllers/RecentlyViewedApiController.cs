using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Controllers
{
    [Authorize()]
    [Route("api/recently-viewed")]
    public class RecentlyViewedApiController : ControllerBase
    {
        private readonly IRepository<ProductRecentlyViewed> _productRecentlyViewedRepository;
        private readonly IWorkContext _workContext;

        public RecentlyViewedApiController(
            IRepository<ProductRecentlyViewed> productRecentlyViewedRepository,
            IWorkContext workContext)
        {
            _productRecentlyViewedRepository = productRecentlyViewedRepository;
            _workContext = workContext;
        }

        [HttpGet()]
        public async Task<Result> List(int take = 20)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();

            var list = await _productRecentlyViewedRepository.Query()
                .Where(c => c.CustomerId == user.Id)
                .Include(c => c.Product).ThenInclude(x => x.ThumbnailImage)
                .OrderByDescending(c => c.LatestViewedOn)
                .Take(take)
                .Select(x => new GoodsListByRecentlyViewedResult
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Slug = x.Product.Slug,
                    Price = x.Product.Price,
                    OldPrice = x.Product.OldPrice,
                    SpecialPrice = x.Product.SpecialPrice,
                    SpecialPriceStart = x.Product.SpecialPriceStart,
                    SpecialPriceEnd = x.Product.SpecialPriceEnd,
                    IsAllowToOrder = x.Product.IsAllowToOrder,
                    ThumbnailUrl = x.Product.ThumbnailImage.Url,
                    ReviewsCount = x.Product.ReviewsCount,
                    RatingAverage = x.Product.RatingAverage,
                    ShortDescription = x.Product.ShortDescription,
                    IsPublished = x.Product.IsPublished,
                    LatestViewedOn = x.LatestViewedOn,
                    IsFeatured = x.Product.IsFeatured
                })
                .ToListAsync();

            var result = new List<GoodsListByRecentlyViewedGroupRessult>();
            var keys = list.GroupBy(c => c.LatestViewedOn.ToString("yyyy-MM-dd")).Select(c => c.Key).OrderByDescending(c => c);
            foreach (var key in keys)
            {
                result.Add(new GoodsListByRecentlyViewedGroupRessult()
                {
                    LatestViewedOnForDay = key,
                    Items = list.Where(c => c.LatestViewedOn.ToString("yyyy-MM-dd") == key).ToList()
                });
            }
            return Result.Ok(result);
        }

        [HttpDelete()]
        public async Task<Result> Clear()
        {
            var user = await _workContext.GetCurrentUserAsync();
            var list = await _productRecentlyViewedRepository.Query()
                .Where(c => c.CustomerId == user.Id)
                .ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
            }
            await _productRecentlyViewedRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpDelete("{productId:int:min(1)}")]
        public async Task<Result> Remove(int productId)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _productRecentlyViewedRepository.Query()
                .Where(c => c.CustomerId == user.Id && c.ProductId == productId)
                .FirstOrDefaultAsync();
            if (model != null)
            {
                model.IsDeleted = true;
                await _productRecentlyViewedRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }
    }
}
