using System.Text.RegularExpressions;

namespace EducationContentService.Domain.ValueObjects;

/// <summary>
/// Contains properties for compiled regex.
/// </summary>
public static partial class RegexExtensions
{
    [GeneratedRegex(@"\s+")]
    public static partial Regex SpaceRemoveRegex();
}
