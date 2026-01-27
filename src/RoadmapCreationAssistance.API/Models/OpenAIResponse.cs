namespace RoadmapCreationAssistance.API.Models;

public class OpenAIResponse
{
    public string Status { get; set; } = string.Empty;

    public required OpenAIOutput Output { get; set; }
}

public class OpenAIOutput
{
    public IEnumerable<OpenAIContent> Content { get; set; } = [];
}

public class OpenAIContent
{
    public string Text { get; set; } = string.Empty;
}