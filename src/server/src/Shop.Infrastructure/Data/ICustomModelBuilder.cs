using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure.Data
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}
