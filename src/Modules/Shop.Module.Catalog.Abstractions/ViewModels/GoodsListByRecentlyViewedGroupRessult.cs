using System.Collections.Generic;

namespace Shop.Module.Catalog.Abstractions.ViewModels
{
    public class GoodsListByRecentlyViewedGroupRessult
    {
        public string LatestViewedOnForDay { get; set; }

        public IList<GoodsListByRecentlyViewedResult> Items { get; set; } = new List<GoodsListByRecentlyViewedResult>();
    }
}
