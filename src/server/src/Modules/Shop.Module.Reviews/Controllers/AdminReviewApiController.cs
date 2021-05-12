using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Reviews.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/admin-reviews")]
    public class AdminReviewApiController : ControllerBase
    {
        private readonly EntityTypeWithId[] entityTypeIds = new EntityTypeWithId[] { EntityTypeWithId.Product };
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Support> _supportRepository;
        private readonly IWorkContext _workContext;

        public AdminReviewApiController(
            IRepository<Review> reviewRepository,
            IRepository<Support> supportRepository,
            IWorkContext workContext)
        {
            _reviewRepository = reviewRepository;
            _supportRepository = supportRepository;
            _workContext = workContext;
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<AdminReviewListResult>>> Grid([FromBody]StandardTableParam<AdminReviewQueryParam> param)
        {
            var query = _reviewRepository.Query();
            var search = param?.Search;
            if (search != null)
            {
                if (search.EntityTypeId.HasValue)
                {
                    if (!entityTypeIds.Any(c => c == search.EntityTypeId.Value))
                        throw new Exception("参数不支持");
                    query = query.Where(c => c.EntityTypeId == (int)search.EntityTypeId.Value);
                }

                if (search.EntityId.HasValue)
                {
                    query = query.Where(c => c.EntityId == search.EntityId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(c => c.Status == search.Status.Value);
                }

                if (search.Rating.HasValue)
                {
                    query = query.Where(c => c.Rating == search.Rating.Value);
                }

                if (search.IsMedia.HasValue)
                {
                    if (search.IsMedia.Value)
                    {
                        query = query.Where(c => c.Medias.Any());
                    }
                    else
                    {
                        query = query.Where(c => !c.Medias.Any());
                    }
                }
            }

            var result = await query
                .Include(c => c.Medias).ThenInclude(c => c.Media)
                .ToStandardTableResult(param, c => new AdminReviewListResult
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    CreatedOn = c.CreatedOn,
                    Rating = c.Rating,
                    Title = c.Title,
                    ReviewerName = c.ReviewerName,
                    SupportCount = c.SupportCount,
                    EntityId = c.EntityId,
                    EntityTypeId = c.EntityTypeId,
                    IsAnonymous = c.IsAnonymous,
                    Status = c.Status,
                    UserId = c.UserId,
                    MediaUrls = c.Medias.OrderBy(x => x.DisplayOrder).Select(x => x.Media.Url)
                });
            return Result.Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody]AdminReviewUpdateParam param)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _reviewRepository.FirstOrDefaultAsync(id);
            if (model != null)
            {
                model.Status = param.Status;
                model.UpdatedOn = DateTime.Now;
                await _reviewRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _reviewRepository.FirstOrDefaultAsync(id);
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedOn = DateTime.Now;
                await _reviewRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }
    }
}
