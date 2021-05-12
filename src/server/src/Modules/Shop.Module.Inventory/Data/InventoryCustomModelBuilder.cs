using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Inventory.Entities;

namespace Shop.Module.Inventory.Data
{
    public class InventoryCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<StockHistory>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Warehouse>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
