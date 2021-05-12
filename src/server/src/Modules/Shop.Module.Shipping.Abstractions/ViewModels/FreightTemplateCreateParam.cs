using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Shipping.ViewModels
{
    public class FreightTemplateCreateParam
    {
        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public string Note { get; set; }
    }
}
