using Shop.Module.Core.Data;

namespace Shop.Module.MQ
{
    public class QueueKeys : ShopKeys
    {
        /// <summary>
        /// 产品浏览记录消息
        /// </summary>
        public static string ProductView = System + "_product_view";

        /// <summary>
        /// 评论自动审核消息
        /// </summary>
        public static string ReviewAutoApproved = System + "_review_auto_approved";

        /// <summary>
        /// 回复自动审核消息
        /// </summary>
        public static string ReplyAutoApproved = System + "_reply_auto_approved";

        /// <summary>
        /// 收到付款消息
        /// </summary>
        public static string PaymentReceived = System + "_payment_received";
    }
}
