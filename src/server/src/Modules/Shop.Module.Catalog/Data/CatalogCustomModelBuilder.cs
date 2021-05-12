using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;

namespace Shop.Module.Catalog.Data
{
    public class CatalogCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            const string module = "Catalog";

            modelBuilder.Entity<ProductAttributeTemplateRelation>()
                .HasOne(pt => pt.Template)
                .WithMany(p => p.ProductAttributes)
                .HasForeignKey(pt => pt.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductAttributeTemplateRelation>()
                .HasOne(pt => pt.Attribute)
                .WithMany(t => t.ProductTemplates)
                .HasForeignKey(pt => pt.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting("Catalog.ProductPageSize") { Module = module, IsVisibleInCommonSettingPage = true, Value = "10" }
            );

            modelBuilder.Entity<ProductOption>().HasData(
                new ProductOption(1) { Name = "Color", DisplayType = OptionDisplayType.Color, CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn },
                new ProductOption(2) { Name = "Size", CreatedOn = GlobalConfiguration.InitialOn, UpdatedOn = GlobalConfiguration.InitialOn }
            );

            modelBuilder.Entity<Product>().HasIndex(b => b.Sku);
            modelBuilder.Entity<Product>().HasIndex(b => b.Name);
            modelBuilder.Entity<Product>().HasIndex(b => b.Slug);
            modelBuilder.Entity<Product>().HasIndex(b => b.IsPublished);
            modelBuilder.Entity<Product>().HasIndex(b => b.IsDeleted);

            modelBuilder.Entity<Brand>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductAttribute>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductAttributeData>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductAttributeGroup>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductAttributeTemplate>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductAttributeTemplateRelation>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductAttributeValue>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductCategory>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductMedia>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductOption>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductOptionCombination>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductOptionData>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductOptionValue>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductPriceHistory>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Unit>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductRecentlyViewed>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductWishlist>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductLink>().HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<EntityType>().HasData(
                new EntityType() { Id = (int)EntityTypeWithId.Category, Name = EntityTypeWithId.Category.GetDisplayName(), Module = module, IsMenuable = true },
                new EntityType() { Id = (int)EntityTypeWithId.Brand, Name = EntityTypeWithId.Brand.GetDisplayName(), Module = module, IsMenuable = true },
                new EntityType() { Id = (int)EntityTypeWithId.Product, Name = EntityTypeWithId.Product.GetDisplayName(), Module = module, IsMenuable = false }
                );
        }
    }
}
