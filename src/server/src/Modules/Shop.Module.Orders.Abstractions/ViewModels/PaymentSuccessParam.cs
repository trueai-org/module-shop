using Shop.Module.Orders.Models;
using System;

namespace Shop.Module.Orders.ViewModels
{
    public class PaymentReceivedParam
    {
        /// <summary>
        /// OrderId 与 OrderNo 必填其一，如果都填了，则优先使用Id
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// OrderId 与 OrderNo 必填其一，如果都填了，则优先使用Id
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethod? PaymentMethod { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? PaymentFeeAmount { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaymentOn { get; set; }

        /// <summary>
        /// 备注
        /// 标记付款
        /// 微信小程序支付回调通知
        /// </summary>
        public string Note { get; set; }
    }
}
