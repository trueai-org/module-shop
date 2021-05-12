using System.Collections.Generic;

namespace Shop.Module.ShoppingCart.ViewModels
{
    public class CheckedItemParam
    {
        public IList<int> ProductIds { get; set; } = new List<int>();

        public bool IsChecked { get; set; }
    }
}
