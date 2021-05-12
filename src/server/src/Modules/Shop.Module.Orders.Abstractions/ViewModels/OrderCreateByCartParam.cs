using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels
{
    public class OrderCreateByCartParam
    {
        [Required]
        public int ShippingUserAddressId { get; set; }

        /// <summary>
        /// 下单备注
        /// </summary>
        [StringLength(450)]
        public string OrderNote { get; set; }
    }
}
