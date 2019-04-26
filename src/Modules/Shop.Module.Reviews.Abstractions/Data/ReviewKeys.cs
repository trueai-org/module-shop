using Shop.Module.Core.Abstractions.Data;

namespace Shop.Module.Reviews.Abstractions.Data
{
    public class ReviewKeys : ShopKeys
    {
        public const string Module = System + ":review";

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
