using Shop.Module.Core.Entities;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface ISmsSender
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> SendSmsAsync(SmsSend model);

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="captcha"></param>
        /// <returns></returns>
        Task<(bool Success, string Message)> SendCaptchaAsync(string phone, string captcha);
    }
}
