using Shop.Infrastructure.Data;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.ViewModels;
using System.Linq;

namespace Shop.Module.Reviews.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        IQueryable<ReviewListQueryDto> List();
    }
}
