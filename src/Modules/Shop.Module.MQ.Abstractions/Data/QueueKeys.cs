using Shop.Module.Core.Abstractions.Data;

namespace Shop.Module.MQ.Abstractions.Data
{
    public class QueueKeys : ShopKeys
    {
        /// <summary>
        /// 产品浏览记录消息
        /// </summary>
        public const string ProductView = System + "_product_view";

        /// <summary>
        /// 评论自动审核消息
        /// </summary>
        public const string ReviewAutoApproved = System + "_review_auto_approved";

        /// <summary>
        /// 回复自动审核消息
        /// </summary>
        public const string ReplyAutoApproved = System + "_reply_auto_approved";

        /// <summary>
        /// 收到付款消息
        /// </summary>
        public const string PaymentReceived = System + "_payment_received";
    }
}
