using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Progress_BarTest
{
    internal class FileInfo
    {
        [JsonPropertyName("file")]
        public string? DirectLink { get; set; }
        [JsonPropertyName("name")]
        public string? FileName { get; set; }
        [JsonPropertyName("size")]
        public int Size { get; set; }   

    }
}
