using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Infrastructure.Models
{
    public class EntityBaseWithTime : EntityBase
    {
        public bool IsDeleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedOn { get; set; }
    }
}
