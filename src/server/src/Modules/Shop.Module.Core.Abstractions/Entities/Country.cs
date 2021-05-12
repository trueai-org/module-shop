using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shop.Infrastructure.Models;

namespace Shop.Module.Core.Entities
{
    public class Country : EntityBase
    {
        public Country()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        [Required]
        [StringLength(450)]
        public string TwoLetterIsoCode { get; set; }

        [Required]
        public string ThreeLetterIsoCode { get; set; }

        [Required]
        public int NumericIsoCode { get; set; }

        public bool IsBillingEnabled { get; set; }

        public bool IsShippingEnabled { get; set; }

        public bool IsCityEnabled { get; set; } = true;

        public bool IsDistrictEnabled { get; set; } = true;

        public IList<StateOrProvince> StatesOrProvinces { get; set; } = new List<StateOrProvince>();

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
