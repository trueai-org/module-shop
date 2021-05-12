using Shop.Module.Core.Data;

namespace Shop.Module.Catalog.Data
{
    public class CatalogKeys : ShopKeys
    {
        public static string Module = System + ":catalog";

        /// <summary>
        /// 单位缓存
        /// </summary>
        public static string UnitAll = Module + ":unit:all";

        /// <summary>
        /// 品牌缓存
        /// </summary>
        public static string BrandAll = Module + ":brand:all";

        /// <summary>
        /// 分类缓存
        /// </summary>
        public static string CategoryAll = Module + ":category:all";

        /// <summary>
        /// 产品属性缓存
        /// </summary>
        public static string AttributeAll = Module + ":attribute:all";

        /// <summary>
        /// 产品属性模板缓存
        /// </summary>
        public static string TemplateAll = Module + ":template:all";

        /// <summary>
        /// 商品信息缓存
        /// </summary>
        public static string GoodsById = Module + ":goods:";
    }
}
