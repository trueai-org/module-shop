using Shop.Module.Core.Models;
using System.Collections.Generic;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderGetAddressResult
    {
        /// <summary>
        /// OrderAddress Id
        /// </summary>
        public int Id { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public int StateOrProvinceId { get; set; }

        public IList<string> StateOrProvinceIds { get; set; } = new List<string>();

        public int CountryId { get; set; }

        public AddressType AddressType { get; set; }
    }
}
