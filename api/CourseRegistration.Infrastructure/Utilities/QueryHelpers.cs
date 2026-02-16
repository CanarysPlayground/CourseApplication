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
    /// <exception cref="ArgumentNullException">Thrown when pattern is null</exception>
    public static string EscapeLikePattern(string pattern)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        return pattern.Replace("^", "^^")   // Escape the escape character first
                     .Replace("%", "^%")   // Escape wildcard for "any characters"
                     .Replace("_", "^_")   // Escape wildcard for "single character"
                     .Replace("[", "^[")   // Escape character set start (SQL Server)
                     .Replace("]", "^]");  // Escape character set end (SQL Server)
    }
}
