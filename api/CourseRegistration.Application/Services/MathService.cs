namespace CourseRegistration.Application.Services;

/// <summary>
/// Provides mathematical utility operations such as factorial calculation.
/// </summary>
public class MathService : IMathService
{
    // 20! is the largest factorial that fits in a long (Int64)
    private const int MaxFactorialInput = 20;

    /// <summary>
    /// Calculates the factorial of a non-negative integer iteratively.
    /// </summary>
    /// <param name="n">A non-negative integer (0 – 20) whose factorial is to be computed.</param>
    /// <returns>The factorial of <paramref name="n"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="n"/> is negative.</exception>
    /// <exception cref="OverflowException">Thrown when <paramref name="n"/> exceeds 20 and the result would overflow <see cref="long"/>.</exception>
    public long CalculateFactorial(int n)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n), "Factorial is not defined for negative numbers.");

        if (n > MaxFactorialInput)
            throw new OverflowException($"Input {n} is too large; factorial exceeds the range of a 64-bit integer (max supported: {MaxFactorialInput}).");

        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }

        return result;
    }
}
