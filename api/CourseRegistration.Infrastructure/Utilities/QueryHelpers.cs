namespace CourseRegistration.Infrastructure.Utilities;

/// <summary>
/// Utility methods for database query operations
/// </summary>
public static class QueryHelpers
{
    /// <summary>
    /// Escapes SQL LIKE pattern special characters to treat them as literals
    /// Uses ^ as the escape character to avoid conflicts with % and _
    /// </summary>
    /// <param name="pattern">The pattern to escape</param>
    /// <returns>The escaped pattern safe for use in SQL LIKE operations</returns>
    public static string EscapeLikePattern(string pattern)
    {
        return pattern.Replace("^", "^^")
                     .Replace("%", "^%")
                     .Replace("_", "^_");
    }
}
