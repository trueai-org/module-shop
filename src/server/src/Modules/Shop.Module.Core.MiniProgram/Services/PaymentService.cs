using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.V2;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Request;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.Payments.Models;
using Shop.Module.Payments.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Shop.Module.Core.MiniProgram.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger _logger;
        private readonly IWeChatPayClient _client;
        private readonly IOptionsMonitor<MiniProgramOptions> _options;
        private readonly string _apiHost;

        public PaymentService(
            ILogger<PaymentService> logger,
            IWeChatPayClient client,
            IOptionsMonitor<MiniProgramOptions> options,
            IOptionsMonitor<ShopOptions> config)
        {
            _logger = logger;
            _client = client;
            _options = options;
            _apiHost = config.CurrentValue.ApiHost;
        }

        public async Task<PaymentOrderBaseResponse> GeneratePaymentOrder(PaymentOrderRequest request)
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString()
                ?? "127.0.0.1";

            var apiHost = _apiHost;
            var wxRequest = new WeChatPayUnifiedOrderRequest
            {
                Body = request.Subject,
                OutTradeNo = request.OrderNo,
                TotalFee = Convert.ToInt32(request.TotalAmount * 100),
                OpenId = request.OpenId,
                TradeType = "JSAPI",
                //SpbillCreateIp = "127.0.0.1",
                SpBillCreateIp = ip,
                NotifyUrl = $"{apiHost.Trim('/')}/api/mp/pay/notify/{request.OrderNo}",
            };

            var config = _options.CurrentValue;
            var opt = new WeChatPayOptions()
            {
                AppId = config.AppId,
                MchId = config.MchId,
                AppSecret = config.AppSecret,
                Key = config.Key
            };
            var response = await _client.ExecuteAsync(wxRequest, opt);
            if (response?.ReturnCode == WeChatPayCode.Success && response?.ResultCode == WeChatPayCode.Success)
            {
                var req = new WeChatPayAppSdkRequest
                {
                    Package = $"prepay_id={response.PrepayId}"
                };

                // https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=7_7&index=5
                // 将参数(parameter)给 小程序前端 让他调起支付API
                var parameter = await _client.ExecuteAsync(req, opt);

                var json = JsonConvert.SerializeObject(parameter);
                return JsonConvert.DeserializeObject<PaymentOrderResponse>(json);
            }
            throw new Exception(response?.ReturnMsg);
        }
    }
}
