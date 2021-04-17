using System;
using System.Collections.Generic;

namespace Shop.Module.Core.Abstractions.ViewModels
{
    public class UserQueryResult
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// 已启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime? LastActivityOn { get; set; }

        /// <summary>
        /// 管理员备注，仅内部使用
        /// </summary>
        public string AdminRemark { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IEnumerable<int> RoleIds { get; set; } = new List<int>();
    }
}
