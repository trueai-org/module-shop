using Shop.Module.Core.Models;
using Shop.Module.Reviews.Models;

namespace Shop.Module.Reviews.ViewModels
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
