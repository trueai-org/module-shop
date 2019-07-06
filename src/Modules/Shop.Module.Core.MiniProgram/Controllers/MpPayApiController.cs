using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.Notify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop.Module.MQ.Abstractions.Data;
using Shop.Module.MQ.Abstractions.Services;
using Shop.Module.Orders.Abstractions.Events;
using Shop.Module.Orders.Abstractions.Models;
using System;
using System.Threading.Tasks;

namespace Shop.Module.Core.MiniProgram.Controllers
{
    [ApiController]
    [Route("api/mp/pay")]
    [Authorize()]
    public class MpPayApiController : ControllerBase
    {
        private readonly IWeChatPayNotifyClient _client;
        private readonly IMQService _mqService;
        private readonly ILogger _logger;

        public MpPayApiController(
            IWeChatPayNotifyClient client,
            IMQService mqService,
            ILogger<MpPayApiController> logger)
        {
            _client = client;
            _mqService = mqService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("notify/{no}")]
        public async Task<IActionResult> NotifyByOrderNo(string no)
        {
            try
            {
                var notify = await _client.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request);
                if (notify.ReturnCode == "SUCCESS")
                {
                    if (notify.ResultCode == "SUCCESS")
                    {
                        await _mqService.DirectSend(QueueKeys.PaymentReceived, new PaymentReceived()
                        {
                            Note = "微信支付成功结果通知",
                            OrderNo = no,
                            PaymentFeeAmount = int.Parse(notify.TotalFee) / 100M,
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
