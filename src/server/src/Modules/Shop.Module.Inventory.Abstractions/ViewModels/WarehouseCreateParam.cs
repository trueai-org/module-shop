using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Inventory.ViewModels
{
    public class WarehouseCreateParam
    {
        [Required]
        public string Name { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "State or Province is required")]
        public int StateOrProvinceId { get; set; }

        [Required]
        public int CountryId { get; set; }

        public string AdminRemark { get; set; }
    }
}
