using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using System;

namespace Shop.Module.Core.Data
{
    public static class CoreSeedData
    {
        public static void SeedData(ModelBuilder builder)
        {
            builder.Entity<AppSetting>().HasData(
                new AppSetting("Global.AssetVersion") { Module = "Core", IsVisibleInCommonSettingPage = false, Value = "1.0" }
            );

            //builder.Entity<EntityType>().HasData(
            //    new EntityType("Vendor") { AreaName = "Core", RoutingController = "Vendor", RoutingAction = "VendorDetail", IsMenuable = false }
            //);

            builder.Entity<Role>().HasData(
                new Role { Id = (int)RoleWithId.admin, ConcurrencyStamp = "4776a1b2-dbe4-4056-82ec-8bed211d1454", Name = "admin", NormalizedName = "ADMIN" },
                new Role { Id = (int)RoleWithId.customer, ConcurrencyStamp = "00d172be-03a0-4856-8b12-26d63fcf4374", Name = "customer", NormalizedName = "CUSTOMER" },
                new Role { Id = (int)RoleWithId.guest, ConcurrencyStamp = "d4754388-8355-4018-b728-218018836817", Name = "guest", NormalizedName = "GUEST" }
            );

            builder.Entity<User>().HasData(
                new User { Id = (int)UserWithId.System, AccessFailedCount = 0, ConcurrencyStamp = "101cd6ae-a8ef-4a37-97fd-04ac2dd630e4", Email = "system@trueai.org", EmailConfirmed = false, FullName = "System User", IsDeleted = false, LockoutEnabled = true, NormalizedEmail = "SYSTEM@TRUEAI.ORG", NormalizedUserName = "SYSTEM", PasswordHash = "AQAAAAEAACcQAAAAEH9OxxtYF2U/mdHwU0b18pjw1UfBbIFY5wrkLS092drNBcKFM99LmxcBUAv3+CTjNQ==", PhoneNumberConfirmed = false, SecurityStamp = "B3IACJI4YP56GU7Y2Y3TJ63ISYLCQM6W", TwoFactorEnabled = false, UserGuid = new Guid("5f72f83b-7436-4221-869c-1b69b2e23aae"), UserName = "system", IsActive = true, CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn },
                new User { Id = (int)UserWithId.Admin, AccessFailedCount = 0, ConcurrencyStamp = "c83afcbc-312c-4589-bad7-8686bd4754c0", Email = "admin@trueai.org", EmailConfirmed = false, FullName = "Shop Admin", IsDeleted = false, LockoutEnabled = true, NormalizedEmail = "ADMIN@TRUEAI.ORG", NormalizedUserName = "ADMIN", PasswordHash = "AQAAAAEAACcQAAAAECWSPYhRBTHbETKsda3xnUUxDaH++2r3TmeHcn/agfSEOMmQFGhRdyKGlg3qJL+1HA==", PhoneNumberConfirmed = false, SecurityStamp = "d6847450-47f0-4c7a-9fed-0c66234bf61f", TwoFactorEnabled = false, UserGuid = new Guid("ed8210c3-24b0-4823-a744-80078cf12eb4"), UserName = "admin", IsActive = true, CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn }
            );

            builder.Entity<UserRole>().HasData(
                new UserRole { UserId = (int)UserWithId.System, RoleId = 1 },
                new UserRole { UserId = (int)UserWithId.Admin, RoleId = 1 }
            );

            builder.Entity<Country>().HasData(
                new Country() { Id = (int)CountryWithId.China, TwoLetterIsoCode = "CN", ThreeLetterIsoCode = "CHN", NumericIsoCode = 156, Name = "China", IsBillingEnabled = true, IsShippingEnabled = true, IsDistrictEnabled = false, CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn },
                new Country() { Id = (int)CountryWithId.UnitedStates, TwoLetterIsoCode = "US", ThreeLetterIsoCode = "USA", NumericIsoCode = 840, Name = "United States", IsBillingEnabled = false, IsShippingEnabled = false, IsDistrictEnabled = true, CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn }
            );

            //builder.Entity<StateOrProvince>().HasData(
            //    new StateOrProvince(1) { CountryId = 1, Name = "Guang Dong", CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn },
            //    new StateOrProvince(2) { CountryId = 2, Name = "Washington", Code = "WA", CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn }
            //);

            //builder.Entity<Address>().HasData(
            //    new Address(1) { AddressLine1 = "Tian he qu", ContactName = "Zhang San", CountryId = 1, StateOrProvinceId = 1, CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn }
            //);

            builder.Entity<WidgetZone>().HasData(
                new WidgetZone((int)WidgetZoneWithId.HomeFeatured) { Name = "Home Featured", CreatedOn = GlobalConfiguration.InitialOn },
                new WidgetZone((int)WidgetZoneWithId.HomeMainContent) { Name = "Home Main Content", CreatedOn = GlobalConfiguration.InitialOn },
                new WidgetZone((int)WidgetZoneWithId.HomeAfterMainContent) { Name = "Home After Main Content", CreatedOn = GlobalConfiguration.InitialOn }
                );


            builder.Entity<Widget>().HasData(
                new Widget((int)WidgetWithId.CategoryWidget) { Name = "Category Widget", ViewComponentName = "CategoryWidget", CreateUrl = "widget-category-create", EditUrl = "widget-category-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn },
                new Widget((int)WidgetWithId.ProductWidget) { Name = "Product Widget", ViewComponentName = "ProductWidget", CreateUrl = "widget-product-create", EditUrl = "widget-product-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn },
                new Widget((int)WidgetWithId.SimpleProductWidget) { Name = "Simple Product Widget", ViewComponentName = "SimpleProductWidget", CreateUrl = "widget-simple-product-create", EditUrl = "widget-simple-product-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn },
                new Widget((int)WidgetWithId.HtmlWidget) { Name = "Html Widget", ViewComponentName = "HtmlWidget", CreateUrl = "widget-html-create", EditUrl = "widget-html-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn },
                new Widget((int)WidgetWithId.CarouselWidget) { Name = "Carousel Widget", ViewComponentName = "CarouselWidget", CreateUrl = "widget-carousel-create", EditUrl = "widget-carousel-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn },
                //new Widget((int)WidgetWithId.SpaceBarWidget) { Name = "SpaceBar Widget", ViewComponentName = "SpaceBarWidget", CreateUrl = "widget-spacebar-create", EditUrl = "widget-spacebar-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn },
                new Widget((int)WidgetWithId.RecentlyViewedWidget) { Name = "Recently Viewed Widget", ViewComponentName = "RecentlyViewedWidget", CreateUrl = "widget-recently-viewed-create", EditUrl = "widget-recently-viewed-edit", IsPublished = true, CreatedOn = GlobalConfiguration.InitialOn }
                );
        }
    }
}