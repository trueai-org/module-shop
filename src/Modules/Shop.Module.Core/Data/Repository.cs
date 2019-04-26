using Shop.Infrastructure.Data;
using Shop.Infrastructure.Models;

namespace Shop.Module.Core.Data
{
    public class Repository<T> : RepositoryWithTypedId<T, int>, IRepository<T> where T : class, IEntityWithTypedId<int>
    {
        public Repository(ShopDbContext context) : base(context)
        {
        }
    }
}
