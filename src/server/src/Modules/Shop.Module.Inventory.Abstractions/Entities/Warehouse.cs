using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Inventory.Entities
{
    public class Warehouse : EntityBase
    {
        public Warehouse()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public Warehouse(int id) : this()
        {
            Id = id;
        }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public int AddressId { get; set; }

        public Address Address { get; set; }

        public string AdminRemark { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
