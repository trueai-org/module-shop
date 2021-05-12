using System.ComponentModel;

namespace Shop.Module.Core.Models
{
    public enum UserTokenType
    {
        Default = 0,

        [Description("One-Time JWT Token")]
        Disposable = 1
    }
}
