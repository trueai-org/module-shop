using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.ShoppingCart.Entities;

namespace Shop.Module.ShoppingCart.Data
{
    public class ShoppingCartCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<CartItem>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
