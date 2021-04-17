using Shop.Infrastructure.Models;

namespace Shop.Infrastructure.Data
{
    public interface IRepository<T> : IRepositoryWithTypedId<T, int> where T : IEntityWithTypedId<int>
    {
    }
}
