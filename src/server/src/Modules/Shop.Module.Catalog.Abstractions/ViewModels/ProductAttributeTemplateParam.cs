using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductAttributeTemplateParam
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IList<int> AttributeIds { get; set; } = new List<int>();
    }
}
