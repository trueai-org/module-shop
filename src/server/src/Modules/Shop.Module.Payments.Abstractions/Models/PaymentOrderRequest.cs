using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Payments.Models
{
    /// <summary>
    /// 订单支付请求参数
    /// </summary>
    public class PaymentOrderRequest
    {
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// 订单号，32个字符内
        /// </summary>
        [StringLength(32)]
        [Required]
        public string OrderNo { get; set; }

        /// <summary>
        /// 商品简单描述
        /// 天网-商城
        /// 腾讯充值中心-QQ会员充值 浏览器打开的网站主页title名-商品概述
        /// 腾讯-游戏 商家名称-销售商品类目
        /// 小张南山店-超市 店名-销售商品类目
        /// 天天爱消除-游戏充值 应用市场上的APP名字-商品概述
        /// </summary>
        [StringLength(128)]
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// 订单总金额，单位元（最多保留2位小数）
        /// </summary>
        [Required]
        [Range(0.01, 50000)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        public string OpenId { get; set; }
    }
}
