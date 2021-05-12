using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class CountryCreateParam
    {
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

        public bool IsCityEnabled { get; set; }

        public bool IsDistrictEnabled { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }
    }
}
