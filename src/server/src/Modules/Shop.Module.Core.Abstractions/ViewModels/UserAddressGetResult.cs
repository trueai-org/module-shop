using Shop.Module.Core.Models;
using System.Linq;

namespace Shop.Module.Core.ViewModels
{
    public class UserAddressGetResult
    {
        public int UserAddressId { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2
        {
            get
            {
                var parts = new[] { CityName, StateOrProvinceName, ZipCode, CountryName };
                return string.Join(", ", parts.Where(x => !string.IsNullOrEmpty(x)));
            }
        }

        public string ZipCode { get; set; }

        public int StateOrProvinceId { get; set; }

        public string StateOrProvinceName { get; set; }

        public string CityName { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public bool IsDistrictEnabled { get; set; }

        public bool IsCityEnabled { get; set; }

        public AddressType AddressType { get; set; }
    }
}
