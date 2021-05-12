using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Shipments.Entities;

namespace Shop.Module.Shipments.Data
{
    public class ShipmentCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipment>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ShipmentItem>().HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Shipment>().HasIndex(c => c.IsDeleted);
            modelBuilder.Entity<Shipment>().HasIndex(c => c.TrackingNumber);
        }
    }
}
