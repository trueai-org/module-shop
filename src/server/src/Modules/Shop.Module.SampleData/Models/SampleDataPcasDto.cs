using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shop.Module.SampleData.Models
{
    public class SampleDataPcasDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("children")]
        public IList<SampleDataPcasDto> Childrens { get; set; }
    }
}

