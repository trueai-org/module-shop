using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Module.Core.ViewModels
{
    public class UserQueryParam
    {
        public string Name { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// 已启用
        /// </summary>
        public bool? IsActive { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// 联系方式，邮箱/电话
        /// </summary>
        public string Contact { get; set; }

        public IList<int> RoleIds { get; set; } = new List<int>();
    }
}
