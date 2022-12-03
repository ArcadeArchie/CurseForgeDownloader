using System.Text.Json.Serialization;

namespace CurseForgeDownloader.Models;


public class CurseMod
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}