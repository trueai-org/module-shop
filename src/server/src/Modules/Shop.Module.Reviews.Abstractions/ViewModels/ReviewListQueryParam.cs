using Shop.Module.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReviewListQueryParam
    {
        public int EntityId { get; set; }

        public EntityTypeWithId EntityTypeId { get; set; }

        [Range(1, 100)]
        public int Take { get; set; } = 1;
    }
}
