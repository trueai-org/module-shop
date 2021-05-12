using Shop.Module.Orders.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderCreateBaseParam
    {
        public int CustomerId { get; set; }

        public int ShippingUserAddressId { get; set; }

        public int? BillingUserAddressId { get; set; }

        public PaymentType PaymentType { get; set; } = PaymentType.OnlinePayment;

        /// <summary>
        /// 配送方式
        /// </summary>
        public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Free;

        /// <summary>
        /// 配送/运费金额
        /// </summary>
        public decimal ShippingFeeAmount { get; set; }

        /// <summary>
        /// 订单折扣总额（运费券、满减券等）
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 下单备注
        /// </summary>
        [StringLength(450)]
        public string OrderNote { get; set; }

        public IList<OrderCreateBaseItemParam> Items { get; set; } = new List<OrderCreateBaseItemParam>();
    }
}
