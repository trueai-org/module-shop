using Shop.Module.Core.Models;
using System;

namespace Shop.Module.Core.ViewModels
{
    public class ProvinceGetResult
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public int CountryId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public StateOrProvinceLevel Level { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
