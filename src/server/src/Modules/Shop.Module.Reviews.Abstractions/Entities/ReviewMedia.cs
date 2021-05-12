using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;

namespace Shop.Module.Reviews.Entities
{
    public class ReviewMedia : EntityBase
    {
        public ReviewMedia()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int ReviewId { get; set; }

        public Review Review { get; set; }

        public int MediaId { get; set; }

        public Media Media { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
