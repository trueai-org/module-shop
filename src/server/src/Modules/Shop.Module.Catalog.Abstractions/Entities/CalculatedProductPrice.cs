namespace Shop.Module.Catalog.Entities
{
    public class CalculatedProductPrice
    {
        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        /// <summary>
        /// 优惠
        /// </summary>
        public int PercentOfSaving { get; set; }
    }
}
