using Shop.Infrastructure.Models;
using Shop.Module.Catalog.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Inventory.Entities
{
    public class Stock : EntityBase
    {
        public Stock()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int StockQuantity { get; set; }

        /// <summary>
        /// 锁定库存数量
        /// </summary>
        public int LockedStockQuantity { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsEnabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(450)]
        public string Note { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
