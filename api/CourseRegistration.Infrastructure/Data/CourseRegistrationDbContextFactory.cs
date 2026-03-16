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

        // Use SQL Server for migrations with a default connection string
        // This connection string can be overridden at runtime via configuration
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=CourseRegistrationDb;Trusted_Connection=True;MultipleActiveResultSets=true",
            options => options.MigrationsAssembly("CourseRegistration.Infrastructure"));

        return new CourseRegistrationDbContext(optionsBuilder.Options);
    }
}
