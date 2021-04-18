using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Reviews.Abstractions.Models;

namespace Shop.Module.Reviews.Abstractions.ViewModels
{
    public class AdminReviewQueryParam
    {
        public int? EntityId { get; set; }

        public EntityTypeWithId? EntityTypeId { get; set; }

        public ReviewStatus? Status { get; set; }

        /// <summary>
        /// 有图
        /// </summary>
        public bool? IsMedia { get; set; }

        public int? Rating { get; set; }
    }
}
