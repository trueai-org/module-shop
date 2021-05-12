using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.SmsSenderAliyun.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Shop.Module.SmsSenderAliyun.Services
{
    /// <summary>
    /// https://help.aliyun.com/document_detail/101414.html?spm=a2c4g.11186623.6.612.450fbc45kUV6Vu
    /// https://help.aliyun.com/document_detail/101874.html?spm=a2c4g.11186623.2.11.136134fc4GnNvp
    /// https://github.com/aliyun/aliyun-openapi-net-sdk/blob/master/README_zh.md
    /// </summary>
    public class AliyunSmsSenderService : ISmsSender
    {
        const string SEPARATOR = "&";

        private readonly int timeoutInMilliSeconds = 100000;
        private readonly string version = "2017-05-25";
        private readonly string action = "SendSms";
        private readonly string format = "JSON";
        private readonly string domain = "dysmsapi.aliyuncs.com";
        private readonly string regionId;
        private readonly string accessKeyId;
        private readonly string accessKeySecret;
        private readonly bool isTest;

        private readonly ILogger _logger;
        private readonly IRepository<SmsSend> _smsSendRepository;
        private readonly IStaticCacheManager _cacheManager;

        public AliyunSmsSenderService(
            ILoggerFactory loggerFactory,
            IOptionsMonitor<AliyunSmsOptions> options,
            IRepository<SmsSend> smsSendRepository,
            IStaticCacheManager cacheManager)
        {
            _logger = loggerFactory.CreateLogger<AliyunSmsSenderService>();
            _smsSendRepository = smsSendRepository;
            _cacheManager = cacheManager;


            regionId = options.CurrentValue.RegionId;
            accessKeyId = options.CurrentValue.AccessKeyId;
            accessKeySecret = options.CurrentValue.AccessKeySecret;
            isTest = options.CurrentValue.IsTest;
        }

        public async Task<bool> SendSmsAsync(SmsSend model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (isTest || model.IsTest)
                {
                    model.IsTest = true;
                    model.IsSucceed = true;
                    return true;
                }

                var paramers = new Dictionary<string, string>();
                paramers.Add("PhoneNumbers", model.PhoneNumber);
                paramers.Add("SignName", model.SignName);
                paramers.Add("TemplateCode", model.TemplateCode);
                paramers.Add("TemplateParam", model.TemplateParam);
                paramers.Add("AccessKeyId", accessKeyId);

                var url = GetSignUrl(paramers, accessKeySecret);
                var result = await HttpGetAsync(url);
                if (result.StatusCode == 200 && !string.IsNullOrEmpty(result.Response))
                {
                    var message = JsonConvert.DeserializeObject<AliyunSendSmsResult>(result.Response);
                    if (message?.Code == "OK")
                    {
                        model.IsSucceed = true;
                        model.Message = message.Code;
                        model.ReceiptId = message.BizId;
                        return true;
                    }
                    else if (message != null)
                    {
                        //smsRecord.
                        model.Message = message.Message;
                    }
                    else
                    {
                        model.Message = result.Response;
                    }
                }
                else
                {
                    model.Message = "发送短信失败";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"send sms error: {ex.Message}", model);
                if (model != null)
                {
                    model.Message = ex.Message;
                }
            }
            finally
            {
                if (model != null)
                {
                    _logger.LogDebug($"sms: {JsonConvert.SerializeObject(model)}");
                    _smsSendRepository.Add(model);
                    await _smsSendRepository.SaveChangesAsync();
                }
            }
            return false;
        }

        public async Task<(bool Success, string Message)> SendCaptchaAsync(string phone, string captcha)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentNullException(nameof(phone));
            if (string.IsNullOrWhiteSpace(captcha))
                throw new ArgumentNullException(nameof(captcha));
            phone = phone.Trim();
            captcha = captcha.Trim();
            var regex = new Regex(@"^\d{11}$");
            if (!regex.IsMatch(phone))
                return (false, "手机号格式错误");

            var cacheKey = ShopKeys.RegisterPhonePrefix + phone;
            if (_cacheManager.IsSet(cacheKey))
            {
                return (false, "请求频繁，请稍后重试");
            }

            var code = captcha;
            var success = await SendSmsAsync(new SmsSend()
            {
                PhoneNumber = phone,
                Value = code,
                TemplateType = SmsTemplateType.Captcha,
                TemplateCode = "SMS_70055704",
                SignName = "天网",
                TemplateParam = JsonConvert.SerializeObject(new { code }),
            });
            if (success)
            {
                _cacheManager.Set(cacheKey, code, 1);
                return (true, "发送成功");
            }
            return (false, "发送短信验证码失败");
        }

        private static string SignString(string source, string accessSecret)
        {
            using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        private string GetSignUrl(Dictionary<string, string> parameters, string accessSecret)
        {
            var imutableMap = new Dictionary<string, string>(parameters);
            imutableMap.Add("Timestamp", FormatIso8601Date(DateTime.Now));
            imutableMap.Add("SignatureMethod", "HMAC-SHA1");
            imutableMap.Add("SignatureVersion", "1.0");
            imutableMap.Add("SignatureNonce", Guid.NewGuid().ToString());
            imutableMap.Add("Action", action);
            imutableMap.Add("Version", version);
            imutableMap.Add("Format", format);
            imutableMap.Add("RegionId", regionId);

            IDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(imutableMap, StringComparer.Ordinal);
            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach (var p in sortedDictionary)
            {
                canonicalizedQueryString
                    .Append("&")
                    .Append(PercentEncode(p.Key)).Append("=")
                    .Append(PercentEncode(p.Value));
            }

            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append("GET");
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode("/"));
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

            string signature = SignString(stringToSign.ToString(), accessSecret + "&");

            imutableMap.Add("Signature", signature);

            return ComposeUrl(domain, imutableMap);
        }

        private static string FormatIso8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
        }

        private static string ComposeUrl(string endpoint, Dictionary<string, string> parameters)
        {
            StringBuilder urlBuilder = new StringBuilder("");
            urlBuilder.Append("http://").Append(endpoint);
            if (urlBuilder.ToString().IndexOf("?") == -1)
            {
                urlBuilder.Append("/?");
            }
            string query = ConcatQueryString(parameters);
            return urlBuilder.Append(query).ToString();
        }

        private static string ConcatQueryString(Dictionary<string, string> parameters)
        {
            if (null == parameters)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var entry in parameters)
            {
                string key = entry.Key;
                string val = entry.Value;
                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null)
                {
                    sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
                }
                sb.Append("&");
            }

            int strIndex = sb.Length;
            if (parameters.Count > 0)
                sb.Remove(strIndex - 1, 1);
            return sb.ToString();
        }

        private static string PercentEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        private async Task<(int StatusCode, string Response)> HttpGetAsync(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * timeoutInMilliSeconds);
                HttpResponseMessage response = await http.GetAsync(url);
                return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
