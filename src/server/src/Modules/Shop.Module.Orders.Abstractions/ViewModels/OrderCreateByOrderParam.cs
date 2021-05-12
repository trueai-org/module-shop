using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderCreateByOrderParam
    {
        [Required]
        public int ShippingUserAddressId { get; set; }

        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// 下单备注
        /// </summary>
        [StringLength(450)]
        public string OrderNote { get; set; }
    }
}
