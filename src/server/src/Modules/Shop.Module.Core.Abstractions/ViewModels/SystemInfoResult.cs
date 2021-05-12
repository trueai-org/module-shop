using System;
using System.Collections.Generic;

namespace Shop.Module.Core.ViewModels
{
    public partial class SystemInfoResult
    {
        public SystemInfoResult()
        {
            this.Headers = new List<HeaderModel>();
            this.LoadedAssemblies = new List<LoadedAssembly>();
        }

        public string AspNetInfo { get; set; }

        public string IsFullTrust { get; set; }

        public string Version { get; set; }

        public string OperatingSystem { get; set; }

        public DateTime ServerLocalTime { get; set; }

        public string ServerTimeZone { get; set; }

        public DateTime UtcTime { get; set; }

        public string HttpHost { get; set; }

        public IList<HeaderModel> Headers { get; set; }

        public IList<LoadedAssembly> LoadedAssemblies { get; set; }

        public partial class HeaderModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public partial class LoadedAssembly
        {
            public string FullName { get; set; }
            public string Location { get; set; }
            public bool IsDebug { get; set; }
            public DateTime? BuildDate { get; set; }
        }
    }
}
