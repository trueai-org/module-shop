using Shop.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    public class EntityType : EntityBase
    {
        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public bool IsMenuable { get; set; }

        [Required]
        [StringLength(450)]
        public string Module { get; set; }
    }
}
