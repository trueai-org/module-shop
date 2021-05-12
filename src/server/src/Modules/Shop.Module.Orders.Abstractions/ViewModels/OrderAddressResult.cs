using System;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderAddressResult
    {
        public int Id { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string FullAddressLine1
        {
            get
            {
                return $"{StateOrProvinceName} {CityName} {DistrictName} {AddressLine1}";
            }
        }

        public string ZipCode { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public int StateOrProvinceId { get; set; }

        public string StateOrProvinceName { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; }

        public int? DistrictId { get; set; }

        public string DistrictName { get; set; }
    }
}
