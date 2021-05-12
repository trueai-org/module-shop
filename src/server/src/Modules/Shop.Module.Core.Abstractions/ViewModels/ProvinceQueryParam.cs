using Shop.Module.Core.Models;
using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public class ProvinceQueryParam
    {
        public int? ParentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public IList<StateOrProvinceLevel> Level { get; set; } = new List<StateOrProvinceLevel>();
    }
}
