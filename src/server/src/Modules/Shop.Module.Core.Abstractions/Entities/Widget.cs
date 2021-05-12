using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    public class Widget : EntityBase
    {
        public Widget(int id)
        {
            Id = id;
            CreatedOn = DateTime.Now;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public string ViewComponentName { get; set; }

        public string CreateUrl { get; set; }

        public string EditUrl { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
