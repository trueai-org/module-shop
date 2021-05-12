using Shop.Module.Reviews.Models;
using System;

namespace Shop.Module.Reviews.ViewModels
{
    public class AdminReplyListResult
    {
        public int Id { get; set; }

        public string Comment { get; set; }

        public string ReplierName { get; set; }

        public int SupportCount { get; set; }

        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 当前用户回复的用户
        /// </summary>
        public string ToUserName { get; set; }

        public int? ParentId { get; set; }

        public int ReviewId { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 当前用户回复的用户
        /// </summary>
        public int? ToUserId { get; set; }

        public ReplyStatus Status { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
