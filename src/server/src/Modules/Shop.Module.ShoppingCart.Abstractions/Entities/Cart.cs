using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using System;
using System.Collections.Generic;

namespace Shop.Module.ShoppingCart.Entities
{
    public class Cart : EntityBase
    {
        public Cart()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            IsActive = true;
        }

        public int CustomerId { get; set; }

        public User Customer { get; set; }

        public bool IsActive { get; set; }

        public string CouponCode { get; set; }

        public string CouponRuleName { get; set; }

        public string ShippingMethod { get; set; }

        public bool IsProductPriceIncludeTax { get; set; }

        public decimal? ShippingAmount { get; set; }

        public IList<CartItem> Items { get; set; } = new List<CartItem>();

        /// <summary>
        /// Json serialized of shipping form
        /// </summary>
        public string ShippingData { get; set; }

        public string OrderNote { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public int UpdatedById { get; set; }

        public User UpdatedBy { get; set; }
    }
}
