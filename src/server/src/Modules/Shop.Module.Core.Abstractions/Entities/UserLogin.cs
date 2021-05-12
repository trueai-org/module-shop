using Microsoft.AspNetCore.Identity;
using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    public class UserLogin : IdentityUserLogin<int>, IEntityWithTypedId<int>
    {
        public UserLogin()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int Id { get; set; }

        [StringLength(450)]
        public string UnionId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
