using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using System;

namespace Shop.Module.Catalog.Services
{
    public class ProductPricingService : IProductPricingService
    {
        public CalculatedProductPrice CalculateProductPrice(GoodsListResult productThumbnail)
        {
            return CalculateProductPrice(productThumbnail.Price, productThumbnail.OldPrice, productThumbnail.SpecialPrice, productThumbnail.SpecialPriceStart, productThumbnail.SpecialPriceEnd);
        }

        public CalculatedProductPrice CalculateProductPrice(Product product)
        {
            return CalculateProductPrice(product.Price, product.OldPrice, product.SpecialPrice, product.SpecialPriceStart, product.SpecialPriceEnd);
        }

        public CalculatedProductPrice CalculateProductPrice(decimal price, decimal? oldPrice, decimal? specialPrice, DateTime? specialPriceStart, DateTime? specialPriceEnd)
        {
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
}
