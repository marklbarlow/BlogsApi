using System.Text;
using System.Text.RegularExpressions;

namespace BlogsApi.Model;

public static class Extensions
{
    private static readonly Regex ImageRegex = new("<img.*?>", RegexOptions.Compiled);
    private static readonly string[] UnclosedTags = { "img", "input", "br", "hr", "meta", "etc" };

    public static BlogSummary ToSummary(this BlogEntry entry)
    {
        return new BlogSummary
        {
            Id = entry.Id,
            Text = entry.Text.StripImageTags().TruncateHtml(500),
            Timestamp = entry.Timestamp,
            Title = entry.Title,
            UserId = entry.UserId,
            Username = entry.Username
        };
    }

    private static string StripImageTags(this string source)
    {
        return ImageRegex.Replace(source, string.Empty);
    }

    private static string TruncateHtml(this string input, int length)
    {
        if (input.Length < length) return input;

        var capturingTag = false;
        var closing = false;
        var done = false;
        var inTag = false;
        var outputBuilder = new StringBuilder();
        var tagBuilder = new StringBuilder();
        var tagStack = new Stack<string>();

        void CaptureTagIfNeeded()
        {
            if (tagBuilder.Length > 0)
            {
                var tag = tagBuilder.ToString();

                if (!UnclosedTags.Contains(tag))
                {
                    tagStack.Push(tag);
                }

                tagBuilder.Clear();
                capturingTag = false;
            }
        }

        for (var i = 0; i < input.Length && !done; i++)
        {
            switch (input[i])
            {
                case '<':
                {
                    capturingTag = true;
                    inTag = true;
                    closing = false;

                    break;
                }
                case '>':
                {
                    inTag = false;

                    if (closing)
                    {
                        tagStack.Pop();
                        capturingTag = false;
                    }
                    // Not self-closing tag
                    else if (input[i - 1] != '/')
                    {
                        CaptureTagIfNeeded();
                    }

                    break;
                }
                case '/':
                {
                    // Closing tag
                    if (input[i - 1] == '<') closing = true;

                    break;
                }
                case ' ':
                {
                    CaptureTagIfNeeded();

                    if (i > length && !inTag)
                    {
                        done = true;
                    }

                    break;
                }
                default:
                {
                    if (capturingTag) tagBuilder.Append(input[i]);

                    break;
                }
            }

            if (!done)
            {
                outputBuilder.Append(input[i]);
            }
        }

        outputBuilder.Append("...");

        while (tagStack.Count > 0) outputBuilder.Append($"</{tagStack.Pop()}>");

        return outputBuilder.ToString();
    }
}