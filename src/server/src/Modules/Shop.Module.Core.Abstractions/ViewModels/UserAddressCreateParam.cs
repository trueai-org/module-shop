using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels
{
    public class UserAddressCreateParam
    {
        [MaxLength(20)]
        [Required(ErrorMessage = "请输入联系人")]
        public string ContactName { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "请输入手机号码")]
        public string Phone { get; set; }

        [MaxLength(200)]
        [Required(ErrorMessage = "请输入地址")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "请选择省")]
        public int StateOrProvinceId { get; set; }

        [Required(ErrorMessage = "请选择市")]
        public int CityId { get; set; }

        public int? DistrictId { get; set; }

        public bool IsDefault { get; set; }
    }
}
