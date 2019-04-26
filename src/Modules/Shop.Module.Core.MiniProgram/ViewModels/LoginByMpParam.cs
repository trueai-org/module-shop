using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.MiniProgram.ViewModels
{
    public class LoginByMpParam
    {
        [Required]
        public string Code { get; set; }

        public string NickName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
