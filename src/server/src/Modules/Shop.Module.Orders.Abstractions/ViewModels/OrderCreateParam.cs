using Shop.Module.Orders.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderCreateParam
    {
        /// <summary>
        /// 客户
        /// </summary>
        [Required]
        public int CustomerId { get; set; }

        public int? ShippingUserAddressId { get; set; }

        public int? BillingUserAddressId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public ShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// 配送/运费金额
        /// </summary>
        public decimal ShippingFeeAmount { get; set; }

        /// <summary>
        /// 订单明细 产品总额 Sum(ProductPrice * Quantity)
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// 订单明细 折扣总额 Sum(DiscountAmount)
        /// </summary>
        public decimal SubTotalWithDiscount { get; set; }

        /// <summary>
        /// 订单总金额 SubTotal + ShippingFeeAmount - SubTotalWithDiscount - DiscountAmount 
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// 订单折扣总额（运费券、满减券等）
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 下单备注
        /// </summary>
        [StringLength(450)]
        public string OrderNote { get; set; }

        /// <summary>
        /// 管理员备注（仅内部使用）
        /// </summary>
        [StringLength(450)]
        public string AdminNote { get; set; }

        public IList<OrderCreateItemParam> Items { get; set; } = new List<OrderCreateItemParam>();

        public OrderCreateAddressParam BillingAddress { get; set; }

        public OrderCreateAddressParam ShippingAddress { get; set; }
    }
}
