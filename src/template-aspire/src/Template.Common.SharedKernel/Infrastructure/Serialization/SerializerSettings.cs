using Newtonsoft.Json;

namespace Template.Common.SharedKernel.Infrastructure.Serialization;

public static class SerializerSettings
{
    private static readonly Lazy<JsonSerializerSettings> _instance = new(() => new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    });

    public static JsonSerializerSettings Instance => _instance.Value;
}
