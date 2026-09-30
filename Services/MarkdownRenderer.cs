using Markdig;
using Microsoft.AspNetCore.Components;

namespace portfolio.Services;

public static class MarkdownRenderer
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static MarkupString Render(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return new MarkupString(string.Empty);
        }

        var html = Markdown.ToHtml(markdown.Trim(), Pipeline);
        return new MarkupString(html);
    }
}
