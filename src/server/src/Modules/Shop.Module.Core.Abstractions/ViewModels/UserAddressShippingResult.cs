using System;

namespace Shop.Module.Core.ViewModels
{
    public class UserAddressShippingResult
    {
        public int AddressId { get; set; }

        public int UserAddressId { get; set; }

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

        public bool IsDefault { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
