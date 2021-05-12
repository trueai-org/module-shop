using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System;

namespace Shop.Module.Catalog.Services
{
    public interface IProductPricingService
    {
        CalculatedProductPrice CalculateProductPrice(GoodsListResult productThumbnail);

        CalculatedProductPrice CalculateProductPrice(Product product);

        CalculatedProductPrice CalculateProductPrice(decimal price, decimal? oldPrice, decimal? specialPrice, DateTime? specialPriceStart, DateTime? specialPriceEnd);
    }
}
