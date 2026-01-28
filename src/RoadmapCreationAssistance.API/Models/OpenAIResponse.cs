namespace RoadmapCreationAssistance.API.Models;

public class OpenAIResponse
{
    public string Status { get; set; } = string.Empty;

    public List<OpenAIOutput> Output { get; set; } = [];
}

public class OpenAIOutput
{
    public string Type { get; set; } = string.Empty;

    public List<OpenAIContent> Content { get; set; } = [];
}

public class OpenAIContent
{
    public string Text { get; set; } = string.Empty;
}