using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CourseRegistration.Infrastructure.Data;

/// <summary>
/// Design-time factory for CourseRegistrationDbContext to enable migrations
/// </summary>
public class CourseRegistrationDbContextFactory : IDesignTimeDbContextFactory<CourseRegistrationDbContext>
{
    public CourseRegistrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseRegistrationDbContext>();
        
        // Use in-memory database for design-time operations
        optionsBuilder.UseInMemoryDatabase("CourseRegistrationDb");
        
        return new CourseRegistrationDbContext(optionsBuilder.Options);
    }
}
