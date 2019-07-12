namespace Shop.Module.Hangfire.Models
{
    /// <summary>
    /// Hangfire数据库连接类型 MySql = 0,SqlServer = 1,Redis = 2,Memory = 3
    /// </summary>
    public enum ProviderType
    {
        MySql = 0,
        SqlServer = 1,
        Redis = 2,
        Memory = 3
    }
}
