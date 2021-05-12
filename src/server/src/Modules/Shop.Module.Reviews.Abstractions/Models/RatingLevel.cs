using System.ComponentModel;

namespace Shop.Module.Reviews.Models
{
    public enum RatingLevel
    {
        [Description("差评")]
        Bad = 1,
        [Description("中评")]
        Medium = 3,
        [Description("好评")]
        Positive = 5
    }
}
