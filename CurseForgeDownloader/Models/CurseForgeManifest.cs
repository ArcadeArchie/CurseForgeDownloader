using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CurseForgeDownloader.Models
{
    public class CurseForgeManifest
    {
        [JsonIgnore]
        public string FilePath { get; set; } = null!;

        [JsonIgnore]
        public bool FromZip { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("manifestType")]
        public string? ManifestType { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("minecraft")]
        public MinecraftType? Minecraft { get; set; }

        [JsonPropertyName("files")]
        public IEnumerable<CurseManifestFile>? Files { get; set; }
    }

    public class MinecraftType
    {
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("modLoaders")]
        public IEnumerable<Modloaders>? Modloaders { get; set; }
    }

    public class Modloaders
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }
    }

    public class CurseManifestFile
    {
        [JsonPropertyName("projectID")]
        public int ProjectID { get; set; }

        [JsonPropertyName("fileID")]
        public int FileID { get; set; }

        [JsonPropertyName("required")]
        public bool Required { get; set; }
    }

}
