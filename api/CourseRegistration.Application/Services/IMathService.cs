namespace CourseRegistration.Application.Services;

/// <summary>
/// Interface for mathematical utility operations.
/// </summary>
public interface IMathService
{
    /// <summary>
    /// Calculates the factorial of a non-negative integer.
    /// </summary>
    /// <param name="n">A non-negative integer whose factorial is to be computed.</param>
    /// <returns>The factorial of <paramref name="n"/> as a <see cref="long"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="n"/> is negative.</exception>
    /// <exception cref="OverflowException">Thrown when the result exceeds <see cref="long.MaxValue"/>.</exception>
    long CalculateFactorial(int n);
}
