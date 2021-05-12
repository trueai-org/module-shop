using System;
using System.Collections.Generic;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReviewListResult
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public string ReviewerName { get; set; }

        public string Avatar { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedOnForDate { get { return CreatedOn.ToString("yyyy-MM-dd"); } }

        public int ReplieCount { get; set; }

        public int SupportCount { get; set; }

        public IEnumerable<string> MediaUrls { get; set; } = new List<string>();

        public IEnumerable<ReplyListResult> Replies { get; set; } = new List<ReplyListResult>();
    }
}
