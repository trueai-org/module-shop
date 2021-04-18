using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Shop.Infrastructure.Modules
{
    public class ModuleInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        public Assembly Assembly { get; set; }
    }
}
