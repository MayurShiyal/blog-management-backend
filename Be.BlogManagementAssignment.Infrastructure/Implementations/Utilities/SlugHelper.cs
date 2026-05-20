using System.Text.RegularExpressions;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Utilities;

/// <summary>
/// Provides slug generation from human-readable strings.
/// Example: "Web Development" → "web-development"
/// </summary>
public static class SlugHelper
{
    public static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var slug = input.Trim().ToLowerInvariant();

        // Replace spaces and underscores with hyphens
        slug = Regex.Replace(slug, @"[\s_]+", "-");

        // Remove characters that are not alphanumeric or hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);

        // Collapse multiple consecutive hyphens
        slug = Regex.Replace(slug, @"-{2,}", "-");

        // Trim leading/trailing hyphens
        slug = slug.Trim('-');

        return slug;
    }
}
