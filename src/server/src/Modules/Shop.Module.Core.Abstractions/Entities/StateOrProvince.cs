using Shop.Infrastructure.Models;
using Shop.Module.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    /// <summary>
    /// 州/省、市、区、街道
    /// </summary>
    public class StateOrProvince : EntityBase
    {
        public StateOrProvince()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int? ParentId { get; set; }

        public StateOrProvince Parent { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        [StringLength(450)]
        public string Code { get; set; }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public StateOrProvinceLevel Level { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
