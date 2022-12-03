using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CurseForgeDownloader.Models
{
    internal class CurseFilesResponse
    {
        [JsonPropertyName("data")]

        public IEnumerable<CurseFile> Data { get; set; } = null!;
    }
    internal class CurseFilesRequest
    {
        [JsonPropertyName("fileIds")]
        public IEnumerable<int>? FileIds { get; set; } 
    }
}