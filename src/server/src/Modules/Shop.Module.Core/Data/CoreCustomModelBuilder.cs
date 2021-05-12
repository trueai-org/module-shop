using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;

namespace Shop.Module.Core.Data
{
    public class CoreCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSetting>().ToTable("Core_AppSetting");

            modelBuilder.Entity<User>()
                .ToTable("Core_User");

            modelBuilder.Entity<Role>()
                .ToTable("Core_Role");

            modelBuilder.Entity<UserLogin>()
                .ToTable("Core_UserLogin");

            modelBuilder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("Core_UserClaim");
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("Core_RoleClaim");
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });
                b.HasOne(ur => ur.Role).WithMany(x => x.Users).HasForeignKey(r => r.RoleId);
                b.HasOne(ur => ur.User).WithMany(x => x.Roles).HasForeignKey(u => u.UserId);
                b.ToTable("Core_UserRole");
            });

            modelBuilder.Entity<IdentityUserToken<int>>(b =>
            {
                b.ToTable("Core_UserToken");
            });

            modelBuilder.Entity<Entity>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.EntityId);
            });

            modelBuilder.Entity<User>(u =>
            {
                u.HasOne(x => x.DefaultShippingAddress)
                   .WithMany()
                   .HasForeignKey(x => x.DefaultShippingAddressId)
                   .OnDelete(DeleteBehavior.Restrict);

                u.HasOne(x => x.DefaultBillingAddress)
                    .WithMany()
                    .HasForeignKey(x => x.DefaultBillingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserAddress>()
                .HasOne(x => x.User)
                .WithMany(a => a.UserAddresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>(x =>
            {
                x.HasOne(d => d.StateOrProvince)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(d => d.Country)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Address>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Country>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<StateOrProvince>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<UserAddress>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<SmsSend>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<WidgetInstance>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<UserLogin>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Entity>().HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<SmsSend>().HasIndex(c => c.PhoneNumber);
            modelBuilder.Entity<SmsSend>().HasIndex(c => c.IsUsed);
            modelBuilder.Entity<SmsSend>().HasIndex(c => c.IsSucceed);

            modelBuilder.Entity<User>().HasIndex(c => c.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(c => c.PhoneNumber).IsUnique().HasFilter("[PhoneNumber] IS NOT NULL");
            modelBuilder.Entity<User>().HasIndex(c => c.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            CoreSeedData.SeedData(modelBuilder);
        }
    }
}
