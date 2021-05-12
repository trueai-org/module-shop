using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Data;

namespace Shop.Module.Core.Extensions
{
    public class ShopUserStore : UserStore<User, Role, ShopDbContext, int, IdentityUserClaim<int>, UserRole, UserLogin, IdentityUserToken<int>, IdentityRoleClaim<int>>
    {
        public ShopUserStore(ShopDbContext context, IdentityErrorDescriber describer) : base(context, describer)
        {
        }
    }
}
