using System;

namespace Shop.Module.Inventory.ViewModels
{
    public class StockHistoryQueryResult
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public int AdjustedQuantity { get; set; }

        public int StockQuantity { get; set; }

        public string Note { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedById { get; set; }

    }
}
