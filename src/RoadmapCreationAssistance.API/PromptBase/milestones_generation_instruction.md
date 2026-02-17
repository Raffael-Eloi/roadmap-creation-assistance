## Final commands

Given this prompt, I want you to generate milestones with issues based on this roadmap.

**IMPORTANT**:
- The answer needs to be exactly an array of Milestones, because I will deserialize the answer to this model in my .NET application.
- The milestone description property should use markdown and should be very detailed, simulating an Epic in the agile development
- The issue body property should use markdown and should be very detailed, simulating a Story in the agile development
- The ME issue should start with [ME] in the title, the TECH issue should start with [TECH] and the HO issue should start with [HO]
- The instructions should be clear to a Junior Software Engineer or someone that does not have solid experience in the area

Here's my classes:

```csharp
public class Milestone
{
    // Do not populate the Id
    public int Id { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public IEnumerable<Issue> Issues { get; set; } = [];
}
```

```csharp
public class Issue
{
    public required string Title { get; set; }

    public string Body { get; set; } = string.Empty;

    // It should be ONLY ONE of these values: "TECH", "ME", "HO"
    // Example: Labels = ["TECH"]
    public IEnumerable<string> Labels = [];
}
```
