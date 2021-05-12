using Shop.Module.Core.Data;

namespace Shop.Module.Reviews.Data
{
    public class ReviewKeys : ShopKeys
    {
        public static string Module = System + ":review";

        /// <summary>
        /// 启用评论自动审核功能
        /// </summary>
        public const string IsReviewAutoApproved = "IsReviewAutoApproved";

        /// <summary>
        /// 启用回复自动审核功能
        /// </summary>
        public const string IsReplyAutoApproved = "IsReplyAutoApproved";
    }
}
