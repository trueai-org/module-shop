using System;

namespace Shop.Module.Reviews.ViewModels
{
    public class ReviewInfoResult
    {
        public int ReviewsCount { get; set; }

        public int MediasCount { get; set; }

        public decimal RatingAverage
        {
            get
            {
                var rate = 0M;
                if (ReviewsCount > 0)
                {
                    rate = ((1 * Rating1Count) + (2 * Rating2Count) + (3 * Rating3Count) + (4 * Rating4Count) + (5 * Rating5Count)) / (decimal)ReviewsCount;
                }
                return Math.Round(rate, 1);
            }
        }

        public int PositiveRatingPercent
        {
            get
            {
                var rate = 0M;
                if (Rating5Count > 0)
                {
                    rate = (decimal)Rating5Count / ReviewsCount * 100;
                }
                return Convert.ToInt32(Math.Ceiling(rate));
            }
        }

        public int Rating1Count { get; set; }

        public int Rating2Count { get; set; }

        public int Rating3Count { get; set; }

        public int Rating4Count { get; set; }

        public int Rating5Count { get; set; }
    }
}
