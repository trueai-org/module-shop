using Shop.Module.Catalog.Entities;
using System;

namespace Shop.Module.Catalog.ViewModels
{
    public class GoodsListResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public decimal? SpecialPrice { get; set; }

        public DateTime? SpecialPriceStart { get; set; }

        public DateTime? SpecialPriceEnd { get; set; }

        public string ThumbnailUrl { get; set; }

        public int ReviewsCount { get; set; }

        public double? RatingAverage { get; set; }

        public bool IsAllowToOrder { get; set; }

        public CalculatedProductPrice CalculatedProductPrice
        {
            get
            {
                decimal price = Price;
                decimal? oldPrice = OldPrice;
                decimal? specialPrice = SpecialPrice;
                DateTime? specialPriceStart = SpecialPriceStart;
                DateTime? specialPriceEnd = SpecialPriceEnd;

                var percentOfSaving = 0;
                var calculatedPrice = price;

                if (specialPrice.HasValue && specialPriceStart < DateTime.Now && DateTime.Now < specialPriceEnd)
                {
                    calculatedPrice = specialPrice.Value;

                    if (!oldPrice.HasValue || oldPrice < price)
                    {
                        oldPrice = price;
                    }
                }

                if (oldPrice.HasValue && oldPrice.Value > 0 && oldPrice > calculatedPrice)
                {
                    percentOfSaving = (int)(100 - Math.Ceiling((calculatedPrice / oldPrice.Value) * 100));
                }

                return new CalculatedProductPrice
                {
                    Price = calculatedPrice,
                    OldPrice = oldPrice,
                    PercentOfSaving = percentOfSaving
                };
            }
        }

        public string ShortDescription { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFeatured { get; set; }

        public static GoodsListResult FromProduct(Product x)
        {
            return new GoodsListResult
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Price = x.Price,
                OldPrice = x.OldPrice,
                SpecialPrice = x.SpecialPrice,
                SpecialPriceStart = x.SpecialPriceStart,
                SpecialPriceEnd = x.SpecialPriceEnd,
                IsAllowToOrder = x.IsAllowToOrder,
                ThumbnailUrl = x.ThumbnailImage?.Url,
                ReviewsCount = x.ReviewsCount,
                RatingAverage = x.RatingAverage,
                ShortDescription = x.ShortDescription,
                IsPublished = x.IsPublished,
                IsFeatured = x.IsFeatured
            };
        }
    }
}
