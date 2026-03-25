using Xunit;
using CourseRegistration.Application.Services;

namespace CourseRegistration.Tests.Services;

/// <summary>
/// Unit tests for <see cref="MathService"/> factorial calculation.
/// </summary>
public class MathServiceTests
{
    private readonly MathService _mathService = new();

    // ── Base cases ─────────────────────────────────────────────────────────────

    [Fact]
    public void CalculateFactorial_Zero_ReturnsOne()
    {
        // Arrange / Act
        var result = _mathService.CalculateFactorial(0);

        // Assert
        Assert.Equal(1L, result);
    }

    [Fact]
    public void CalculateFactorial_One_ReturnsOne()
    {
        // Arrange / Act
        var result = _mathService.CalculateFactorial(1);

        // Assert
        Assert.Equal(1L, result);
    }

    // ── Typical positive values ────────────────────────────────────────────────

    [Theory]
    [InlineData(2, 2L)]
    [InlineData(3, 6L)]
    [InlineData(4, 24L)]
    [InlineData(5, 120L)]
    [InlineData(10, 3628800L)]
    [InlineData(15, 1307674368000L)]
    [InlineData(20, 2432902008176640000L)]
    public void CalculateFactorial_PositiveInput_ReturnsCorrectResult(int n, long expected)
    {
        // Arrange / Act
        var result = _mathService.CalculateFactorial(n);

        // Assert
        Assert.Equal(expected, result);
    }

    // ── Error paths ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void CalculateFactorial_NegativeInputs_ThrowsArgumentOutOfRangeException(int n)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _mathService.CalculateFactorial(n));
    }

    [Theory]
    [InlineData(21)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void CalculateFactorial_LargeInputs_ThrowsOverflowException(int n)
    {
        // Arrange / Act / Assert
        Assert.Throws<OverflowException>(() => _mathService.CalculateFactorial(n));
    }
}
