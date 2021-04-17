using Microsoft.EntityFrameworkCore.Storage;
using Shop.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Data
{
    public interface IRepositoryWithTypedId<T, TId> where T : IEntityWithTypedId<TId>
    {
        IQueryable<T> Query();

        void Add(T entity);

        void AddRange(IList<T> entities);

        IDbContextTransaction BeginTransaction();

        int SaveChanges();

        Task<int> SaveChangesAsync();

        void Remove(T entity);

        Task<T> FirstOrDefaultAsync(TId id);

        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
    }
}
