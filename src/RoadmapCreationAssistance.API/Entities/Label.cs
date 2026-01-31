namespace RoadmapCreationAssistance.API.Entities;

public class Label
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Color { get; set; }

    public static IEnumerable<Label> GenerateDefaultLabels()
    {
        return
        [
            new Label()
            {
                Name = "TECH",
                Description = "Technical implementation",
                Color = "416BB8"
            },
            new Label()
            {
                Name = "ME",
                Description = "Mindset Evolution — Reflection and reasoning",
                Color = "33D631"
            },
            new Label()
            {
                Name = "HO",
                Description = "Hands-On — Small practical challenges",
                Color = "FFE638"
            },
        ];
    }
}