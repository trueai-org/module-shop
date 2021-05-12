using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Data;

namespace Shop.Module.Core.Extensions
{
    public class ShopRoleStore: RoleStore<Role, ShopDbContext, int, UserRole, IdentityRoleClaim<int>>
    {
        public ShopRoleStore(ShopDbContext context) : base(context)
        {
        }
    }
}
