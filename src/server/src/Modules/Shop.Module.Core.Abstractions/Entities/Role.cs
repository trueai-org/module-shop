using Microsoft.AspNetCore.Identity;
using Shop.Infrastructure.Models;
using System.Collections.Generic;

namespace Shop.Module.Core.Entities
{
    public class Role : IdentityRole<int>, IEntityWithTypedId<int>
    {
        public IList<UserRole> Users { get; set; } = new List<UserRole>();
    }
}
