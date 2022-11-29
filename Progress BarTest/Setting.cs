using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Progress_BarTest
{
    internal class Setting
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        [JsonPropertyName("link")]
        public  string? Link { get; set; }
        [JsonPropertyName("x86")]
        public bool IsX86 { get; set; }
        [JsonPropertyName("version")]
        public string? Version { get; set; }
    }
}
