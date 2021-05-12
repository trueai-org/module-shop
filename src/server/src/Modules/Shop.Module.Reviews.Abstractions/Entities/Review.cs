using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Reviews.Models;
using Shop.Module.Reviews.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Reviews.Entities
{
    public class Review : EntityBase
    {
        public Review()
        {
            Status = ReviewStatus.Pending;
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int UserId { get; set; }

        public User User { get; set; }

        [StringLength(450)]
        public string Title { get; set; }

        [StringLength(450)]
        public string Comment { get; set; }

        public int Rating { get; set; }

        [StringLength(450)]
        public string ReviewerName { get; set; }

        public ReviewStatus Status { get; set; }

        public int EntityTypeId { get; set; }

        public int EntityId { get; set; }

        /// <summary>
        /// 评论来源Id 例如:订单Id
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// 评论来源类型 Order = 0
        /// </summary>
        public ReviewSourceType? SourceType { get; set; }

        public bool IsAnonymous { get; set; }

        public int SupportCount { get; set; }

        /// <summary>
        /// 是否由系统自动生成的评论
        /// </summary>
        public bool IsSystem { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IList<Reply> Replies { get; protected set; } = new List<Reply>();

        public IList<ReviewMedia> Medias { get; protected set; } = new List<ReviewMedia>();

        public IList<Support> Supports { get; protected set; } = new List<Support>();
    }
}
