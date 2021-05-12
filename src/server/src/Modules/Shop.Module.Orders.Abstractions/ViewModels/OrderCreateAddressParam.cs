using Shop.Module.Core.Models;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderCreateAddressParam
    {
        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public int StateOrProvinceId { get; set; }

        public int CountryId { get; set; }

        public AddressType AddressType { get; set; }
    }
}
