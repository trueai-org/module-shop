using System.Collections.Generic;

namespace Shop.Infrastructure.Web.StandardTable
{
    public class StandardTableResult<T>
    {
        public IEnumerable<T> List { get; set; }
        public StandardTablePagination Pagination { get; set; }
    }
}
