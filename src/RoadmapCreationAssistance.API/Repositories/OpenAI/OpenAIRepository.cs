using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Extensions;
using RoadmapCreationAssistance.API.Repositories.OpenAI.Models;

namespace RoadmapCreationAssistance.API.Repositories.OpenAI;

public sealed class OpenAIRepository(IConfiguration configuration) : IOpenAIRepository
{
    public async Task<string> GetResponse(string input, string openAiToken)
    {
        HttpClient httpClient = CreateHttpClient(openAiToken);

        OpenAIInputRequest request = new(input);
        HttpContent content = request.ToJsonContent();

        HttpResponseMessage response = await httpClient.PostAsync($"/v1/responses", content);
        OpenAIResponse openAIResponse = await response.DeserializeAsync<OpenAIResponse>();
        
        OpenAIOutput? output = openAIResponse.Output.FirstOrDefault(x => x.Type == "message");
        if (output is null)
            throw new InvalidOperationException("No valid output found in OpenAI response.");

        OpenAIContent? openAIContent = output.Content.FirstOrDefault();
        if (openAIContent is null)
            throw new InvalidOperationException("No content found in OpenAI output.");

        return openAIContent.Text;
    }

    private HttpClient CreateHttpClient(string openAiToken)
    {
        HttpClient httpClient = new();
        string baseUrl = GetBaseUrl();
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(180);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiToken);
        return httpClient;
    }

    private string GetBaseUrl()
    {
        return configuration["OpenAIApiBaseUrl"]!;
    }
}