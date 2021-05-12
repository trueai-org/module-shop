using System;

namespace Shop.Module.Catalog.ViewModels
{
    public class GoodsListByRecentlyViewedResult : GoodsListResult
    {
        public DateTime LatestViewedOn { get; set; }
    }
}
