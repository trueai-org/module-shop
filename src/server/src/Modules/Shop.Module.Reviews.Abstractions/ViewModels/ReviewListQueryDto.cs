using Shop.Module.Reviews.Models;
using System;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReviewListQueryDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public int Rating { get; set; }

        public string ReviewerName { get; set; }

        public ReviewStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public int EntityTypeId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string EntitySlug { get; set; }
    }
}
