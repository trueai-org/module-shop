using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Web.StandardTable
{
    public static class StandardTableExtension
    {
        public static async Task<StandardTableResult<TResult>> ToStandardTableResult<TModel, TResult>(this IQueryable<TModel> query, StandardTableParam param, Expression<Func<TModel, TResult>> selector)
        {
            if (param.Pagination == null)
            {
                param.Pagination = new StandardTablePagination()
                {
                    Current = 1,
                    PageSize = 10
                };
            }

            if (param.Pagination.Current < 1)
            {
                param.Pagination.Current = 1;
            }
            if (param.Pagination.PageSize <= 0)
            {
                param.Pagination.PageSize = 10;
            }

            var totalRecord = query.Count();

            if (!string.IsNullOrWhiteSpace(param.Sort?.Predicate))
            {
                var field = param.Sort.Predicate.Trim().Substring(0, 1).ToUpper() + param.Sort.Predicate.Trim().Substring(1);
                var any = typeof(TModel).GetProperties().Any(c => c.Name.Equals(field));
                if (!any)
                    throw new Exception("The sort field does not exist");
                query = query.OrderByName(field, param.Sort.Reverse);
            }
            else
            {
                var field = "Id";
                var any = typeof(TModel).GetProperties().Any(c => c.Name == field);
                if (any)
                    query = query.OrderByName("Id", true);
            }

            var items = await query
                .Skip((param.Pagination.Current - 1) * param.Pagination.PageSize)
                .Take(param.Pagination.PageSize)
                .Select(selector).ToListAsync();

            return new StandardTableResult<TResult>
            {
                List = items,
                Pagination = new StandardTablePagination()
                {
                    Total = totalRecord,
                    PageSize = param.Pagination.PageSize,
                    Current = param.Pagination.Current
                }
            };
        }

        // ToLits() before Select() to loaded related many-to-many entity
        public static async Task<StandardTableResult<TResult>> ToStandardTableResultNoProjection<TModel, TResult>(this IQueryable<TModel> query, StandardTableParam param, Expression<Func<TModel, TResult>> selector)
        {
            if (param.Pagination == null)
            {
                param.Pagination = new StandardTablePagination()
                {
                    Current = 1,
                    PageSize = 10
                };
            }

            if (param.Pagination.Current < 1)
            {
                param.Pagination.Current = 1;
            }
            if (param.Pagination.PageSize <= 0)
            {
                param.Pagination.PageSize = 10;
            }

            var totalRecord = query.Count();

            if (!string.IsNullOrWhiteSpace(param.Sort?.Predicate))
            {
                var field = param.Sort.Predicate.Trim().Substring(0, 1).ToUpper() + param.Sort.Predicate.Trim().Substring(1);
                var any = typeof(TModel).GetProperties().Any(c => c.Name.Equals(field));
                if (!any)
                    throw new Exception("The sort field does not exist");
                query = query.OrderByName(field, param.Sort.Reverse);
            }
            else
            {
                var field = "Id";
                var any = typeof(TModel).GetProperties().Any(c => c.Name == field);
                if (any)
                    query = query.OrderByName("Id", true);
            }

            var items = await query
                .Skip((param.Pagination.Current - 1) * param.Pagination.PageSize)
                .Take(param.Pagination.PageSize)
                .ToListAsync();

            return new StandardTableResult<TResult>
            {
                List = items.AsQueryable().Select(selector),
                Pagination = new StandardTablePagination()
                {
                    Total = totalRecord,
                    PageSize = param.Pagination.PageSize,
                    Current = param.Pagination.Current
                }
            };
        }
    }
}