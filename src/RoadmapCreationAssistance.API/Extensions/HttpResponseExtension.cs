using RoadmapCreationAssistance.API.JsonSerialization;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Extensions;

public static class HttpResponseExtension
{
    public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage httpResponse)
    {
        if (!httpResponse.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"HTTP request failed with status code {(int)httpResponse.StatusCode} ({httpResponse.StatusCode}).");

        string responseJson = await httpResponse.Content.ReadAsStringAsync();
        T? response = JsonSerializer.Deserialize<T>(responseJson, JsonSerializationOptions.Default);

        return response ?? throw new JsonException(
            $"Failed to deserialize response to {typeof(T).Name}. Response body was empty or invalid.");
    }
}