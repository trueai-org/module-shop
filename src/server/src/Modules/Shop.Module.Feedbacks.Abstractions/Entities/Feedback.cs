using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Feedbacks.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Feedbacks.Entities
{
    public class Feedback : EntityBase
    {
        public Feedback()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int? UserId { get; set; }

        public User User { get; set; }

        public string Contact { get; set; }

        [StringLength(450)]
        public string Title { get; set; }

        [StringLength(450)]
        public string Content { get; set; }

        public FeedbackType Type { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
