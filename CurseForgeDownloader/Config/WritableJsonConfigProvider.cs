using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration.Json;

namespace CurseForgeDownloader.Config;


internal class WritableJsonConfigProvider<TConfig> : JsonConfigurationProvider where TConfig : class, new()
{
    private readonly PropertyInfo[] _modelProperties;
    public WritableJsonConfigProvider(WritableJsonConfigSource source) : base(source)
    {
        _modelProperties = typeof(TConfig).GetProperties();
    }

    public override void Set(string key, string? value)
    {
        base.Set(key, value);
        var fullPath = base.Source.FileProvider!.GetFileInfo(base.Source.Path!).PhysicalPath;
        string json = File.ReadAllText(fullPath);

        var jsonObj = JsonSerializer.Deserialize<TConfig>(json);
        if (jsonObj == null)
            jsonObj = new TConfig();

        var prop = _modelProperties.FirstOrDefault(x => x.Name.Contains(key));
        prop.SetValue(jsonObj, value);
        File.WriteAllText(fullPath, JsonSerializer.Serialize(jsonObj));
    }
}