using Shop.Module.Core.Models;
using Shop.Module.Reviews.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReviewAddParam
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "请输入评论，且长度不能超过400")]
        [StringLength(400)]
        public string Comment { get; set; }

        public int EntityId { get; set; }

        public EntityTypeWithId EntityTypeId { get; set; } = EntityTypeWithId.Product;

        /// <summary>
        /// 评论来源Id 例如:订单Id
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// 评论来源类型 Order = 0
        /// </summary>
        public ReviewSourceType? SourceType { get; set; }

        public bool IsAnonymous { get; set; }

        [MaxLength(9)]
        public IList<int> MediaIds { get; set; } = new List<int>();
    }
}
