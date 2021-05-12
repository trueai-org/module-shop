namespace Shop.Module.Core.Models
{
    /// <summary>
    /// 州/省、市、区、街道 类型
    /// </summary>
    public enum StateOrProvinceLevel
    {
        Default = 0, //州、省、直辖市、自治区
        City = 1, //市
        District = 2, //区县
        Street = 3 //街道、乡镇
    }
}
