using Shop.Module.Core.Entities;
using Shop.Module.Core.Data;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Repositories;
using Shop.Module.Reviews.ViewModels;
using System.Linq;

namespace Shop.Module.Reviews.Data
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ShopDbContext context) : base(context)
        {
        }

        public IQueryable<ReviewListQueryDto> List()
        {
            var items = DbSet.Join(Context.Set<Entity>(),
                r => new { key1 = r.EntityId, key2 = r.EntityTypeId },
                u => new { key1 = u.EntityId, key2 = u.EntityTypeId },
                (r, u) => new ReviewListQueryDto
                {
                    EntityTypeId = r.EntityTypeId,
                    Id = r.Id,
                    ReviewerName = r.ReviewerName,
                    Rating = r.Rating,
                    Title = r.Title,
                    Comment = r.Comment,
                    Status = r.Status,
                    CreatedOn = r.CreatedOn,
                    EntityName = u.Name,
                    EntitySlug = u.Slug
                });

            return items;
        }
    }
}
