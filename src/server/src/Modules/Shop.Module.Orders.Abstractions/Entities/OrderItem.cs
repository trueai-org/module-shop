using Newtonsoft.Json;
using Shop.Infrastructure.Models;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using System;

namespace Shop.Module.Orders.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public int OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

        public int ProductId { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }

        /// <summary>
        /// 产品价格（原价）（允许修改）
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 产品名称（快照）
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品图片（快照）
        /// </summary>
        public string ProductMediaUrl { get; set; }

        /// <summary>
        /// 数量（允许修改）
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 已发货数量
        /// </summary>
        public int ShippedQuantity { get; set; }

        /// <summary>
        /// 折扣小计（折扣总金额）（允许修改）
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 金额小计（产品价格*数量 - 折扣总金额）（允许修改）
        /// </summary>
        public decimal ItemAmount { get; set; }

        /// <summary>
        /// 重量小计
        /// </summary>
        public decimal ItemWeight { get; set; }

        /// <summary>
        /// 备注、折扣备注
        /// 例如：满2件8折；满3件6折；活动价90；6折；活动1xxx，活动2xxx
        /// 活动信息
        /// </summary>
        public string Note { get; set; }

        public int CreatedById { get; set; }

        [JsonIgnore]
        public User CreatedBy { get; set; }

        public int UpdatedById { get; set; }

        [JsonIgnore]
        public User UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
