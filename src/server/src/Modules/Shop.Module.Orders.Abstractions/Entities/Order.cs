using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            OrderStatus = OrderStatus.New;
            No = NoGen.Instance.GenOrderNo();
        }

        public long No { get; set; }

        public int CustomerId { get; set; }

        [JsonIgnore] // To simplify the json stored in order history
        public User Customer { get; set; }

        public int? ShippingAddressId { get; set; }

        public OrderAddress ShippingAddress { get; set; }

        public int? BillingAddressId { get; set; }

        public OrderAddress BillingAddress { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// 运输状态
        /// </summary>
        public ShippingStatus? ShippingStatus { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippedOn { get; set; }

        /// <summary>
        /// 收货时间
        /// </summary>
        public DateTime? DeliveredOn { get; set; }

        /// <summary>
        /// 买家确认收货结束时间（卖家发货后生成 T+7X，买家可延长确认收货时间，最大发货后时间+60d）
        /// </summary>
        public DateTime? DeliveredEndOn { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus? RefundStatus { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        public string RefundReason { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundOn { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public ShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// 配送/运费金额
        /// </summary>
        public decimal ShippingFeeAmount { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethod? PaymentMethod { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PaymentFeeAmount { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaymentOn { get; set; }

        /// <summary>
        /// 买家支付结束时间（买家剩余支付时间 T+120mX，买家下单时生成）
        /// </summary>
        public DateTime? PaymentEndOn { get; set; }

        /// <summary>
        /// 优惠码、折扣代码
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 优惠码、折扣代码规则名称
        /// </summary>
        public string CouponRuleName { get; set; }

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

        /// <summary>
        /// 交易关闭/交易取消原因
        /// 可以选择的理由有：
        /// 1、未及时付款
        /// 2、买家不想买了
        /// 3、买家信息填写错误，重新拍
        /// 4、恶意买家/同行捣乱
        /// 5、缺货
        /// 6、买家拍错了
        /// 7、同城见面交易
        /// ...
        /// </summary>
        [StringLength(450)]
        public string CancelReason { get; set; }

        /// <summary>
        /// 挂起原因
        /// 冻结中的订单、纠纷中的订单、退款中的订单
        /// </summary>
        [StringLength(450)]
        public string OnHoldReason { get; set; }

        /// <summary>
        /// 交易关闭/交易取消时间
        /// </summary>
        public DateTime? CancelOn { get; set; }

        public int CreatedById { get; set; }

        [JsonIgnore]
        public User CreatedBy { get; set; }

        public int UpdatedById { get; set; }

        [JsonIgnore]
        public User UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public void AddOrderItem(OrderItem item)
        {
            item.Order = this;
            OrderItems.Add(item);
        }
    }
}
