using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shop.Infrastructure.Models;

namespace Shop.Module.Core.Entities
{
    public class Address : EntityBase
    {
        public Address()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public Address(int id) : this()
        {
            Id = id;
        }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        [Required]
        public int StateOrProvinceId { get; set; }

        public StateOrProvince StateOrProvince { get; set; }

        [Required]
        public int CountryId { get; set; }

        public Country Country { get; set; }

        public IList<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
