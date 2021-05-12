using Shop.Module.Reviews.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Reviews.ViewModels
{
    public class AdminReviewListResult
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public string ReviewerName { get; set; }

        public DateTime CreatedOn { get; set; }

        public int SupportCount { get; set; }

        public IEnumerable<string> MediaUrls { get; set; } = new List<string>();

        public int UserId { get; set; }

        public ReviewStatus Status { get; set; }

        public int EntityTypeId { get; set; }

        public int EntityId { get; set; }

        public bool IsAnonymous { get; set; }
    }
}
