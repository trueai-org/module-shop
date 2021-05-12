using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.ViewModels
{
    public class ProductCloneParam
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        /// <summary>
        /// 复制图片
        /// </summary>
        public bool IsCopyImages { get; set; }

        /// <summary>
        /// 复制库存 
        /// </summary>
        public bool IsCopyStock { get; set; }
    }
}
