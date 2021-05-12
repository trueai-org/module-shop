using Shop.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    public class EmailSend : EntityBase
    {
        public EmailSend()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        [Required]
        public string From { get; set; }

        /// <summary>
        /// 接收
        /// </summary>
        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; }

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
        /// 是否发送成功
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// 发送成功或失败的消息
        /// </summary>
        public string Message { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
