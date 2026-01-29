using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoadmapCreationAssistance.API.JsonSerialization
{
    public static class JsonSerializationOptions
    {
        public static JsonSerializerOptions Default => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
