using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Module.Reviews.Services
{
    public class Reply : EntityBase
    {
        public Reply()
        {
            Status = ReplyStatus.Pending;
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }
        public int? ParentId { get; set; }

        public Reply Parent { get; set; }

        public int ReviewId { get; set; }

        public Review Review { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        [StringLength(450)]
        public string Comment { get; set; }

        [StringLength(450)]
        public string ReplierName { get; set; }

        /// <summary>
        /// 当前用户回复的用户
        /// </summary>
        [ForeignKey("ToUser")]
        public int? ToUserId { get; set; }

        /// <summary>
        /// 当前用户回复的用户
        /// </summary>
        [ForeignKey("ToUserId")]
        public User ToUser { get; set; }

        /// <summary>
        /// 当前用户回复的用户
        /// </summary>
        [StringLength(450)]
        public string ToUserName { get; set; }

        public ReplyStatus Status { get; set; }

        public bool IsAnonymous { get; set; }

        public int SupportCount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IList<Reply> Childrens { get; set; } = new List<Reply>();
    }
}
