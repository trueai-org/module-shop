using Shop.Module.Core.Entities;
using Shop.Module.Core.ViewModels;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// 验证并获取最后一条验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="captcha"></param>
        /// <returns></returns>
        Task<SmsSend> ValidateGetLastSms(string phone, string captcha);
    }
}
