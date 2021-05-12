using Microsoft.AspNetCore.Identity;
using Shop.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Module.Core.Entities
{
    public class User : IdentityUser<int>, IEntityWithTypedId<int>, IExtendableObject
    {
        public User()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public Guid UserGuid { get; set; }

        /// <summary>
        /// 全名（昵称）
        /// </summary>
        [Required]
        [StringLength(450)]
        public string FullName { get; set; }

        public IList<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

        public UserAddress DefaultShippingAddress { get; set; }

        public int? DefaultShippingAddressId { get; set; }

        public UserAddress DefaultBillingAddress { get; set; }

        public int? DefaultBillingAddressId { get; set; }

        public string RefreshTokenHash { get; set; }

        public IList<UserRole> Roles { get; set; } = new List<UserRole>();

        public string Culture { get; set; }

        /// <inheritdoc />
        public string ExtensionData { get; set; }

        /// <summary>
        /// 已启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        [StringLength(450)]
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime? LastActivityOn { get; set; }

        [StringLength(450)]
        public string AvatarUrl { get; set; }

        [ForeignKey("Avatar")]
        public int? AvatarId { get; set; }

        [ForeignKey("AvatarId")]
        public Media Avatar { get; set; }

        /// <summary>
        /// 管理员备注，仅内部使用
        /// </summary>
        [StringLength(450)]
        public string AdminRemark { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
