using FluentAssertions;
using Moq;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

internal class PromptProviderShould
{
    private IPromptProvider promptProvider;

    [SetUp]
    public void Setup()
    {
        promptProvider = new PromptProvider();
    }

    [Test]
    public async Task Get_Prompt_Base()
    {
        #region Arrange

        string language = "Portuguese-BR";

        #endregion

        #region Act

        string result = await promptProvider.GetRoadmapBaseAsync(language, string.Empty);

        #endregion

        #region Assert

        result.Should().Contain($"📌 Prompt: Software Engineering Confidence Roadmap");
        result.Should().Contain($"All documentation, milestones, issues and code must be written **in {language}**");

        #endregion
    }

    [Test]
    public async Task Get_Prompt_Base_With_Domain_Definition()
    {
        #region Arrange

        string domainDefinition = "CRUD of Car Managament";
        #endregion

        #region Act

        string result = await promptProvider.GetRoadmapBaseAsync("English-US", domainDefinition);

        #endregion

        #region Assert

        result.Should().Contain($"The context is {domainDefinition}");
        result.Should().Contain(domainDefinition);

        #endregion
    }

    [Test]
    public async Task Get_Milestone_Instructions()
    {
        #region Arrange

        string language = "Portuguese-BR";

        #endregion

        #region Act

        string result = await promptProvider.GetMilestoneInstructionAsync(language, string.Empty);

        #endregion

        #region Assert

        result.Should().Contain($"📌 Prompt: Software Engineering Confidence Roadmap");
        result.Should().Contain($"Given this prompt, I want you to generate milestones with issues");
        result.Should().Contain($"All documentation, milestones, issues and code must be written **in {language}**");

        #endregion
    }
}