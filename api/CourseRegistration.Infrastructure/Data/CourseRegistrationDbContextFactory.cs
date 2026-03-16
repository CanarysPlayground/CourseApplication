using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CourseRegistration.Infrastructure.Data;

/// <summary>
/// Design-time factory for CourseRegistrationDbContext to support EF Core migrations
/// </summary>
public class CourseRegistrationDbContextFactory : IDesignTimeDbContextFactory<CourseRegistrationDbContext>
{
    /// <summary>
    /// Creates a new instance of CourseRegistrationDbContext for design-time operations
    /// </summary>
    public CourseRegistrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseRegistrationDbContext>();

        // Use SQL Server for migrations
        // Connection string can be overridden via environment variable
        var connectionString = Environment.GetEnvironmentVariable("MIGRATION_CONNECTION_STRING")
            ?? "Server=localhost,1433;Database=CourseRegistrationDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true";

        optionsBuilder.UseSqlServer(
            connectionString,
            options => options.MigrationsAssembly("CourseRegistration.Infrastructure"));

        return new CourseRegistrationDbContext(optionsBuilder.Options);
    }
}
