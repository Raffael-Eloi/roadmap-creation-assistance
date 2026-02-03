using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Extensions;
using RoadmapCreationAssistance.API.Repositories.OpenAI.Models;
using System.Net.Http.Headers;

namespace RoadmapCreationAssistance.API.Repositories.OpenAI;

public sealed class OpenAIRepository(IHttpClientFactory httpClientFactory) : IOpenAIRepository
{
    public const string HttpClientName = "OpenAI";

    public async Task<string> GetResponse(string input, string openAiToken)
    {
        HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiToken);

        OpenAIInputRequest request = new(input);
        HttpContent content = request.ToJsonContent();

        OpenAIResponse? openAIResponse = null;

        try
        {
            HttpResponseMessage response = await httpClient.PostAsync("/v1/responses", content);
            openAIResponse = await response.DeserializeAsync<OpenAIResponse>();
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("Error occurred while sending request to OpenAI API.", ex);
        }

        OpenAIOutput? output = openAIResponse.Output.FirstOrDefault(x => x.Type == "message");
        if (output is null)
            throw new InvalidOperationException("No valid output found in OpenAI response.");

        OpenAIContent? openAIContent = output.Content.FirstOrDefault();
        if (openAIContent is null)
            throw new InvalidOperationException("No content found in OpenAI output.");

        if (string.IsNullOrWhiteSpace(openAIContent.Text))
            throw new InvalidOperationException("OpenAI content text is empty.");

        return openAIContent.Text;
    }
}