using System.Collections.Generic;

namespace Shop.Module.ShoppingCart.ViewModels
{
    public class DeleteItemParam
    {
        public IList<int> ProductIds { get; set; } = new List<int>();
    }
}
