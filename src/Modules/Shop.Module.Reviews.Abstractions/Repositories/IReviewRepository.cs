using Shop.Infrastructure.Data;
using Shop.Module.Reviews.Abstractions.Entities;
using Shop.Module.Reviews.Abstractions.ViewModels;
using System.Linq;

namespace Shop.Module.Reviews.Abstractions.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        IQueryable<ReviewListQueryDto> List();
    }
}
