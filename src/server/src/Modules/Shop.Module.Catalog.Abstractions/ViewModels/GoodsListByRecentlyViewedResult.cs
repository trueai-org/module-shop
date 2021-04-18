using System;

namespace Shop.Module.Catalog.Abstractions.ViewModels
{
    public class GoodsListByRecentlyViewedResult : GoodsListResult
    {
        public DateTime LatestViewedOn { get; set; }
    }
}
