using System.Text;
using System.Text.Json;
using RoadmapCreationAssistance.API.JsonSerialization;

namespace RoadmapCreationAssistance.API.Extensions;

public static class HttpContentExtensions
{
    public static HttpContent ToJsonContent<T>(this T obj)
    {
        string json = JsonSerializer.Serialize(obj, JsonSerializationOptions.Default);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static HttpContent ToJsonContent(this string input) =>
        new StringContent(input, Encoding.UTF8, "application/json");
}