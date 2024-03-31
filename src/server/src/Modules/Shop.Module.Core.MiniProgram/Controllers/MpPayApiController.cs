using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.V2;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Notify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.MQ;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Models;

namespace Shop.Module.Core.MiniProgram.Controllers
{
    /// <summary>
    /// 微信支付API控制器，用于处理微信支付相关的通知和请求。
    /// </summary>
    [ApiController]
    [Route("api/mp/pay")]
    [Authorize()]
    public class MpPayApiController : ControllerBase
    {
        private readonly IWeChatPayNotifyClient _client;
        private readonly IMQService _mqService;
        private readonly ILogger _logger;
        private readonly MiniProgramOptions _options;

        public MpPayApiController(
            IWeChatPayNotifyClient client,
            IMQService mqService,
            ILogger<MpPayApiController> logger,
            IOptionsMonitor<MiniProgramOptions> options)
        {
            _client = client;
            _mqService = mqService;
            _logger = logger;
            _options = options.CurrentValue;
        }

        /// <summary>
        /// 接收并处理微信支付成功后的异步通知。验证通知的真实性，并且处理业务逻辑，如更新订单状态、记录支付信息等。
        /// </summary>
        /// <param name="no">订单编号，用于标识支付通知对应的订单。</param>
        /// <returns>返回处理结果。若处理成功，返回微信服务器期望的成功响应，否则返回无内容响应。</returns>
        [AllowAnonymous]
        [HttpPost("notify/{no}")]
        public async Task<IActionResult> NotifyByOrderNo(string no)
        {
            try
            {
                var config = _options;
                var opt = new WeChatPayOptions()
                {
                    AppId = config.AppId,
                    MchId = config.MchId,
                    AppSecret = config.AppSecret,
                    Key = config.Key
                };

                var notify = await _client.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request, opt);
                if (notify.ReturnCode == "SUCCESS")
                {
                    if (notify.ResultCode == "SUCCESS")
                    {
                        await _mqService.Send(QueueKeys.PaymentReceived, new PaymentReceived()
                        {
                            Note = "微信支付成功结果通知",
                            OrderNo = no,
                            PaymentFeeAmount = notify.TotalFee / 100M,
                            PaymentMethod = PaymentMethod.WeChat,
                            PaymentOn = DateTime.ParseExact(notify.TimeEnd, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture)
                        });
                        return WeChatPayNotifyResult.Success;
                    }
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
            finally
            {
                _logger.LogInformation("参数：{@no}", no);
            }
        }
    }
}