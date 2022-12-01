
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CurseForgeDownloader.Config;

internal class WritableJsonConfigSource : JsonConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        this.EnsureDefaults(builder);
        return new WritableJsonConfigProvider<AppConfig>(this);
    }
}