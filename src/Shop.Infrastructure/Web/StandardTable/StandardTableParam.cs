namespace Shop.Infrastructure.Web.StandardTable
{
    public class StandardTableParam
    {
        public StandardTablePagination Pagination { get; set; }

        public StandardSearch Search { get; set; }

        public StandardSort Sort { get; set; }
    }

    public class StandardTableParam<T> : StandardTableParam
    {
        public new T Search { get; set; }
    }
}
