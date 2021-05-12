using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Shipping.Entities;

namespace Shop.Module.Shipping.Data
{
    public class ShippingCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FreightTemplate>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<PriceAndDestination>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
