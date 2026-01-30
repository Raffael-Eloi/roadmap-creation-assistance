using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.JsonSerialization;
using System.Text;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Repositories;

public sealed class OpenAIRepository(HttpClient httpClient, IConfiguration configuration) : IOpenAIRepository
{
    public async Task<string> GetResponse(string input, string openAiToken)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(180);
        string baseUrl = configuration["OpenAIApiBaseUrl"]!;
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiToken);
        OpenAIInputRequest request = new OpenAIInputRequest(input);
        string json = JsonSerializer.Serialize(request, JsonSerializationOptions.Default);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/v1/responses", content);
        string responseJson = await response.Content.ReadAsStringAsync();
        OpenAIResponse? openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseJson, JsonSerializationOptions.Default);
        if (openAIResponse is null)
            return string.Empty;

        OpenAIOutput? output = openAIResponse.Output.FirstOrDefault(x => x.Type == "message");
        if (output is null)
            return string.Empty;

        return output.Content.FirstOrDefault()!.Text;
    }

    internal sealed class OpenAIInputRequest(string input)
    {
        public string Model => "gpt-5-nano-2025-08-07";

        public string Input { get; set; } = input;
    }

    internal sealed class OpenAIResponse
    {
        public string Status { get; set; } = string.Empty;

        public List<OpenAIOutput> Output { get; set; } = [];
    }

    internal sealed class OpenAIOutput
    {
        public string Type { get; set; } = string.Empty;

        public List<OpenAIContent> Content { get; set; } = [];
    }

    internal sealed class OpenAIContent
    {
        public string Text { get; set; } = string.Empty;
    }
}