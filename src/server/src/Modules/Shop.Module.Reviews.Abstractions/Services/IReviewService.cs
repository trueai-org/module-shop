using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Reviews.Abstractions.Models;
using System.Threading.Tasks;

namespace Shop.Module.Reviews.Services.Abstractions
{
    public interface IReviewService
    {
        /// <summary>
        /// 自动好评
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityTypeId"></param>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        Task ReviewAutoGood(int entityId, EntityTypeWithId entityTypeId, int? sourceId, ReviewSourceType? sourceType);
    }
}
