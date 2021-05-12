using Shop.Module.Core.Models;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReviewInfoParam
    {
        public int EntityId { get; set; }

        public EntityTypeWithId EntityTypeId { get; set; }
    }
}
