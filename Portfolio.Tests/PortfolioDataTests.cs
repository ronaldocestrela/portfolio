using portfolio.Data;

namespace Portfolio.Tests;

public class PortfolioDataTests
{
    [Fact]
    public void YearsOfExperience_ShouldUseOldestExperienceStartDate()
    {
        var data = new PortfolioData
        {
            Experience =
            [
                new ExperienceItem { StartDate = "03/2023", Role = "Front-end" },
                new ExperienceItem { StartDate = "01/2021", EndDate = "12/2023", Role = "Full-Stack" }
            ]
        };

        Assert.Equal(5, data.YearsOfExperience);
    }

    [Fact]
    public void YearsOfExperience_ShouldReturnZero_WhenNoExperienceDatesExist()
    {
        var data = new PortfolioData
        {
            Experience =
            [
                new ExperienceItem { Role = "Front-end", Company = "Example" }
            ]
        };

        Assert.Equal(0, data.YearsOfExperience);
    }

    [Fact]
    public void ExperienceTech_ShouldRemoveDuplicates_KeepingUniqueValues()
    {
        var experience = new ExperienceItem
        {
            Tech = [".NET", "C#", ".NET", "C#", "Blazor"]
        };

        Assert.Equal([".NET", "C#", "Blazor"], experience.DistinctTech);
    }

    [Fact]
    public void ProjectStack_ShouldRemoveDuplicates_KeepingUniqueValues()
    {
        var project = new ProjectItem
        {
            Stack = ["React", "Node.js", "React", "SQL", "Node.js"]
        };

        Assert.Equal(["React", "Node.js", "SQL"], project.DistinctStack);
    }

    [Fact]
    public void ProjectImages_ShouldKeepOnlyFiveImages_AndHonorDisplayImage()
    {
        var project = new ProjectItem
        {
            Images = ["/img/1.jpg", "/img/2.jpg", "/img/3.jpg", "/img/4.jpg", "/img/5.jpg", "/img/6.jpg"],
            DisplayImage = "/img/4.jpg"
        };

        Assert.Equal(["/img/1.jpg", "/img/2.jpg", "/img/3.jpg", "/img/4.jpg", "/img/5.jpg"], project.ValidImages);
        Assert.Equal("/img/4.jpg", project.DisplayImageUrl);
    }

    [Fact]
    public void ProjectImages_ShouldFallbackToFirstImage_WhenNoDisplayImageIsSelected()
    {
        var project = new ProjectItem
        {
            Images = ["/img/1.jpg", "/img/2.jpg", "/img/3.jpg"]
        };

        Assert.Equal("/img/1.jpg", project.DisplayImageUrl);
    }

    [Fact]
    public void ProjectDetailFields_ShouldStayIndependent_WhenTheyAreEmpty()
    {
        var project = new ProjectItem
        {
            Description = "Descrição geral do projeto"
        };

        project.NormalizeDetailSections();

        Assert.Equal(string.Empty, project.CaseStudySummary);
        Assert.Equal(string.Empty, project.Problem);
        Assert.Equal(string.Empty, project.Solution);
        Assert.Equal(string.Empty, project.Result);
    }

    [Fact]
    public void MarkdownRenderer_ShouldTransformBasicMarkdownIntoHtml()
    {
        var html = portfolio.Services.MarkdownRenderer.Render("# Título\n\n**Resumo**\n\n- item 1\n- item 2");

        Assert.Contains("<h1", html.Value);
        Assert.Contains("<strong>Resumo</strong>", html.Value);
        Assert.Contains("<ul>", html.Value);
        Assert.Contains("<li>item 1</li>", html.Value);
    }
}
