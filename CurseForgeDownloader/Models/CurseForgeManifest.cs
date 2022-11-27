using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseForgeDownloader.Models
{
    public class CurseForgeManifest
    {
        public string FilePath { get; set; } = null!;
        public bool FromZip { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? ManifestType { get; set; }
        public string? Author { get; set; }

        public MinecraftType? Minecraft { get; set; }
        public IEnumerable<ModFile>? Files { get; set; }
    }

    public class MinecraftType
    {
        public string? Version { get; set; }
        public IEnumerable<Modloaders>? Modloaders { get; set; }
    }

    public class Modloaders
    {
        public string? Id { get; set; }
        public bool Primary { get; set; }
    }
    public class ModFile
    {
        public int ProjectID { get; set; }
        public int FileID { get; set; }
        public bool Required { get; set; }
    }
}
