using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Orders.Data;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Events;

namespace Shop.Module.Orders.Data
{
    public class OrderCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            const string module = "Orders";

            modelBuilder.Entity<Order>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<OrderAddress>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<OrderHistory>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Order>(u =>
            {
                u.HasOne(x => x.ShippingAddress)
                .WithMany()
                .HasForeignKey(x => x.ShippingAddressId);
            });

            modelBuilder.Entity<Order>()
                .HasIndex(b => b.No)
                .IsUnique();

            modelBuilder.Entity<Order>(u =>
            {
                u.HasOne(x => x.BillingAddress)
                .WithMany()
                .HasForeignKey(x => x.BillingAddressId);
            });

            var cfg = GlobalConfiguration.Configuration;

            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting(OrderKeys.OrderAutoCanceledTimeForMinute)
                {
                    Module = module,
                    IsVisibleInCommonSettingPage = true,
                    Value = cfg[$"{nameof(OrderOptions)}:{nameof(OrderOptions.OrderAutoCanceledTimeForMinute)}"],
                    Type = typeof(int).FullName,
                    Note = "订单下单后超时自动取消订单时间（单位：分钟）"
                },
                new AppSetting(OrderKeys.OrderAutoCompleteTimeForMinute)
                {
                    Module = module,
                    IsVisibleInCommonSettingPage = true,
                    Value = cfg[$"{nameof(OrderOptions)}:{nameof(OrderOptions.OrderAutoCompleteTimeForMinute)}"],
                    Type = typeof(int).FullName,
                    Note = "订单支付后超时自动完成订单时间（买家未在指定的时间内确认收货,则系统自动确认收货完成订单，单位：分钟）"
                },
                new AppSetting(OrderKeys.OrderCompleteAutoReviewTimeForMinute)
                {
                    Module = module,
                    IsVisibleInCommonSettingPage = true,
                    Value = cfg[$"{nameof(OrderOptions)}:{nameof(OrderOptions.OrderCompleteAutoReviewTimeForMinute)}"],
                    Type = typeof(int).FullName,
                    Note = "订单完成后超时自动好评时间（买家未在指定的时间内评价,则系统自动好评，单位：分钟）"
                });
        }
    }
}
