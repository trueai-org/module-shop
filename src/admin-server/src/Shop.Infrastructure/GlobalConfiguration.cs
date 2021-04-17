using Microsoft.Extensions.Configuration;
using Shop.Infrastructure.Localization;
using Shop.Infrastructure.Modules;
using System;
using System.Collections.Generic;

namespace Shop.Infrastructure
{
    public static partial class GlobalConfiguration
    {
        /// <summary>
        /// 数据初始化时间
        /// </summary>
        public static DateTime InitialOn = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public static IList<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>();

        public static IList<Culture> Cultures { get; set; } = new List<Culture>();

        public static string DefaultCulture => "en-US";

        public static string WebRootPath { get; set; }

        public static string ContentRootPath { get; set; }

        public static IConfiguration Configuration { get; set; }

        public const string NoImage = "no-image.png";
    }
}
