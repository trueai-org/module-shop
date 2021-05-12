using Shop.Infrastructure.Models;
using System;

namespace Shop.Module.Catalog.Entities
{
    public class Unit : EntityBase
    {
        public Unit()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
