using Shop.Infrastructure.Models;
using Shop.Module.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    public class SmsSend : EntityBase
    {
        public SmsSend()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        /// <summary>
        /// 接收短信的手机号码。
        /// 国内短信：11位手机号码，例如15951955195。
        /// 国际/港澳台消息：国际区号+号码，例如85200000000。
        /// </summary>
        [StringLength(450)]
        [Required]
        public string PhoneNumber { set; get; }

        /// <summary>
        /// 发送的值（验证码等）
        /// </summary>
        [StringLength(450)]
        public string Value { get; set; }

        /// <summary>
        /// 短信签名名称。请在控制台签名管理页面签名名称一列查看。
        /// </summary>
        [StringLength(450)]
        public string SignName { get; set; }

        /// <summary>
        /// 短信模板类型
        /// </summary>
        public SmsTemplateType? TemplateType { get; set; }

        /// <summary>
        /// 短信模板ID。请在控制台模板管理页面模板CODE一列查看。
        /// SMS_153055065
        /// </summary>
        [StringLength(450)]
        public string TemplateCode { set; get; }

        /// <summary>
        /// 短信模板变量对应的实际值，JSON格式。
        /// {"code":"1111"}
        /// </summary>
        public string TemplateParam { set; get; }

        /// <summary>
        /// 外部流水扩展字段。
        /// </summary>
        [StringLength(450)]
        public string OutId { get; set; }

        /// <summary>
        /// 发送回执ID。
        /// </summary>
        [StringLength(450)]
        public string ReceiptId { get; set; }

        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// 是否发送成功
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// 是否为测试短信
        /// 测试短信不真正发送短信，仅生成发送记录
        /// </summary>
        public bool IsTest { get; set; }

        /// <summary>
        /// 发送成功或失败的消息
        /// </summary>
        public string Message { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
