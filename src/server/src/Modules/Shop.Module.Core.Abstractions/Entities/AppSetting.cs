using Shop.Infrastructure.Models;
using Shop.Module.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Entities
{
    public class AppSetting : EntityBaseWithTypedId<string>
    {
        public AppSetting(string id)
        {
            Id = id;
        }

        [StringLength(450)]
        public string Module { get; set; }

        public AppSettingFormatType FormatType { get; set; } = AppSettingFormatType.None;

        /// <summary>
        /// Shop.Module.Hangfire.Models.HangfireOptions, Shop.Module.Hangfire, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
        /// </summary>
        [StringLength(450)]
        public string Type { get; set; }

        public string Value { get; set; }

        public bool IsVisibleInCommonSettingPage { get; set; }

        [StringLength(450)]
        public string Note { get; set; }
    }
}
