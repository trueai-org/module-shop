using System.ComponentModel;

namespace Shop.Module.Feedbacks.Models
{
    public enum FeedbackType
    {
        [Description("商品相关")]
        Product = 0,
        [Description("物流相关")]
        Logistics = 1,
        [Description("客户服务")]
        Customer = 2,
        [Description("优惠活动")]
        Discounts = 3,
        [Description("功能异常")]
        Dysfunction = 4,
        [Description("产品建议")]
        ProductProposal = 5,
        [Description("其他")]
        Other = 6
    }
}
