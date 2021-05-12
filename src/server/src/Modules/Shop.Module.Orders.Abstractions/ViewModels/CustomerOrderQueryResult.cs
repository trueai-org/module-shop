using Shop.Infrastructure.Helpers;
using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.Orders.ViewModels
{
    public class CustomerOrderQueryResult
    {
        public int Id { get; set; }

        public string No { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int? ShippingAddressId { get; set; }

        public int? BillingAddressId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// 运输状态
        /// </summary>
        public ShippingStatus? ShippingStatus { get; set; }

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
        public string OrderNote { get; set; }

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
        public string CancelReason { get; set; }

        /// <summary>
        /// 交易关闭/交易取消时间
        /// </summary>
        public DateTime? CancelOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string OrderStatusString { get { return OrderStatus.GetDisplayName(); } }

        public decimal SubTotal { get { return Items.Sum(x => x.Quantity * x.ProductPrice); } }

        public string SubTotalString { get { return SubTotal.ToString("C"); } }

        public OrderAddressResult Address { get; set; }

        public IEnumerable<CustomerOrderItemQueryResult> Items { get; set; } = new List<CustomerOrderItemQueryResult>();

        /// <summary>
        /// 商品总数
        /// </summary>
        public int ItemsTotal { get; set; }

        /// <summary>
        /// 商品品类数量
        /// </summary>
        public int ItemsCount { get; set; }

        /// <summary>
        /// 买家支付结束时间（买家剩余支付时间 T+120mX，买家下单时生成）
        /// </summary>
        public DateTime? PaymentEndOn { get; set; }

        /// <summary>
        /// 买家支付结束时间 
        /// </summary>
        public int PaymentEndOnForSecond
        {
            get
            {
                if (PaymentEndOn.HasValue && PaymentEndOn > DateTime.Now &&
                    (OrderStatus == OrderStatus.New || OrderStatus == OrderStatus.PendingPayment || OrderStatus == OrderStatus.PaymentFailed))
                {
                    var totalSec = (PaymentEndOn - DateTime.Now).Value.TotalSeconds;
                    if (totalSec > 0)
                    {
                        return Convert.ToInt32(totalSec);
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 买家确认收货结束时间（卖家发货后生成 T+7X）
        /// </summary>
        public DateTime? DeliveredEndOn { get; set; }

        /// <summary>
        /// 买家确认收货结束时间
        /// </summary>
        public int DeliveredEndOnForSecond
        {
            get
            {
                if (DeliveredEndOn.HasValue && DeliveredEndOn > DateTime.Now &&
                    (OrderStatus == OrderStatus.Shipping || OrderStatus == OrderStatus.Shipped) &&
                    (ShippingStatus == Models.ShippingStatus.NoShipping || ShippingStatus == Models.ShippingStatus.PartiallyShipped || ShippingStatus == Models.ShippingStatus.Shipped))
                {
                    var totalSec = (DeliveredEndOn - DateTime.Now).Value.TotalSeconds;
                    if (totalSec > 0)
                    {
                        return Convert.ToInt32(totalSec);
                    }
                }
                return 0;
            }
        }
    }
}
