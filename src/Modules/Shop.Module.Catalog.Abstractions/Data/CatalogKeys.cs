using Shop.Module.Core.Abstractions.Data;

namespace Shop.Module.Catalog.Abstractions.Data
{
    public class CatalogKeys : ShopKeys
    {
        public const string Module = System + ":catalog";

        /// <summary>
        /// 单位缓存
        /// </summary>
        public const string UnitAll = Module + ":unit:all";

        /// <summary>
        /// 品牌缓存
        /// </summary>
        public const string BrandAll = Module + ":brand:all";

        /// <summary>
        /// 分类缓存
        /// </summary>
        public const string CategoryAll = Module + ":category:all";

        /// <summary>
        /// 产品属性缓存
        /// </summary>
        public const string AttributeAll = Module + ":attribute:all";

        /// <summary>
        /// 产品属性模板缓存
        /// </summary>
        public const string TemplateAll = Module + ":template:all";

        /// <summary>
        /// 商品信息缓存
        /// </summary>
        public const string GoodsById = Module + ":goods:";
    }
}
