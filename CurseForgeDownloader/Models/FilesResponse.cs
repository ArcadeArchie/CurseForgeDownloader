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
    internal class CurseFile
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("modId")]
        public int ModId { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = null!;

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = null!;
        
        [JsonPropertyName("downloadUrl")]
        public string? DownloadUrl { get; set; }
    }
}