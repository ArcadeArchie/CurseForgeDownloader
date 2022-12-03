using System.Text.Json.Serialization;

namespace CurseForgeDownloader.Models;

public class CurseFile
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