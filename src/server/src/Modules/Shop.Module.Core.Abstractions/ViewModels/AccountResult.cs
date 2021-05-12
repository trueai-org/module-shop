using System;

namespace Shop.Module.Core.ViewModels
{
    public class AccountResult
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// 全名（昵称）
        /// </summary>
        public string FullName { get; set; }

        public string Avatar { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Email { get; set; }

        public string Culture { get; set; }

        public string LastIpAddress { get; set; }

        public DateTime? LastLoginOn { get; set; }

        public DateTime? LastActivityOn { get; set; }

        public string AdminRemark { get; set; }

        public int NotifyCount { get; set; }

        public string Name => FullName;
        public string Phone => PhoneNumber;
    }
}
