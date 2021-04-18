namespace Shop.Module.Inventory.Areas.Inventory.ViewModels
{
    public class WarehouseQueryResult
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public int StateOrProvinceId { get; set; }

        public int CountryId { get; set; }

        public string AdminRemark { get; set; }

        public int AddressId { get; set; }
    }
}
