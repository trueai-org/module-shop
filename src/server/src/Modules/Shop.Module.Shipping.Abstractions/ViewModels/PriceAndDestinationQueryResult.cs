using Shop.Module.Core.Models;
using System;

namespace Shop.Module.Shipping.ViewModels
{
    public class PriceAndDestinationQueryResult
    {
        public int Id { get; set; }

        public int FreightTemplateId { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public int? StateOrProvinceId { get; set; }

        public StateOrProvinceLevel? StateOrProvinceLevel { get; set; }

        public string StateOrProvinceName { get; set; }

        public decimal MinOrderSubtotal { get; set; }

        public decimal ShippingPrice { get; set; }

        public string Note { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
