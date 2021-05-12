using System.ComponentModel;

namespace Shop.Module.Core.Models
{
    /// <summary>
    /// 用户双因子验证类型
    /// </summary>
    public enum UserTwoFactorType
    {
        [Description("手机")]
        Phone = 0,
        [Description("邮箱")]
        Email = 1
    }
}
