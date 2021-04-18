using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Reviews.Abstractions.Models;

namespace Shop.Module.Reviews.Abstractions.ViewModels
{
    public class ReviewQueryParam
    {
        public int EntityId { get; set; }

        public EntityTypeWithId EntityTypeId { get; set; }

        /// <summary>
        /// 有图
        /// </summary>
        public bool? IsMedia { get; set; }

        public RatingLevel? RatingLevel { get; set; }
    }
}
