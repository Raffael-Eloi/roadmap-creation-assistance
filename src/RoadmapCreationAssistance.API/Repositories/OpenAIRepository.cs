using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Extensions;

namespace RoadmapCreationAssistance.API.Repositories;

public sealed class OpenAIRepository(HttpClient httpClient, IConfiguration configuration) : IOpenAIRepository
{
    public async Task<string> GetResponse(string input, string openAiToken)
    {
        string baseUrl = GetBaseUrl();

        AddOpenAiTokenToHttpClient(openAiToken);

        OpenAIInputRequest request = new(input);

        HttpContent content = request.ToJsonContent();

        HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/v1/responses", content);
        
        OpenAIResponse openAIResponse = await response.DeserializeAsync<OpenAIResponse>();
        
        OpenAIOutput? output = openAIResponse.Output.FirstOrDefault(x => x.Type == "message");
        if (output is null)
            return string.Empty;

        return output.Content.FirstOrDefault()!.Text;
    }

    private void AddOpenAiTokenToHttpClient(string openAiToken)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(180);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiToken);
    }

    private string GetBaseUrl()
    {
        return configuration["OpenAIApiBaseUrl"]!;
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