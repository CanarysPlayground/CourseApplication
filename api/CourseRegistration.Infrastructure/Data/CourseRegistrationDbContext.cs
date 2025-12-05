using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Infrastructure.Data;

/// <summary>
/// Database context for the Course Registration application
/// </summary>
public class CourseRegistrationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the CourseRegistrationDbContext
    /// </summary>
    /// <param name="options">Database context options</param>
    public CourseRegistrationDbContext(DbContextOptions<CourseRegistrationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Students DbSet
    /// </summary>
    public DbSet<Student> Students { get; set; } = null!;

    /// <summary>
    /// Courses DbSet
    /// </summary>
    public DbSet<Course> Courses { get; set; } = null!;

    /// <summary>
    /// Registrations DbSet
    /// </summary>
    public DbSet<Registration> Registrations { get; set; } = null!;

    /// <summary>
    /// Configures the model relationships and constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(student => student.StudentId);
            entity.HasIndex(student => student.Email).IsUnique();
            entity.Property(student => student.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(student => student.LastName).IsRequired().HasMaxLength(50);
            entity.Property(student => student.Email).IsRequired().HasMaxLength(256);
            entity.Property(student => student.PhoneNumber).HasMaxLength(20);
            entity.Property(student => student.DateOfBirth).IsRequired();
            entity.Property(student => student.CreatedAt).IsRequired();
            entity.Property(student => student.UpdatedAt).IsRequired();
            entity.Property(student => student.IsActive).IsRequired();

            // Configure relationships
            entity.HasMany(student => student.Registrations)
                  .WithOne(registration => registration.Student)
                  .HasForeignKey(registration => registration.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Course entity
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(course => course.CourseId);
            entity.Property(course => course.CourseName).IsRequired().HasMaxLength(100);
            entity.Property(course => course.Description).HasMaxLength(500);
            entity.Property(course => course.InstructorName).IsRequired().HasMaxLength(100);
            entity.Property(course => course.StartDate).IsRequired();
            entity.Property(course => course.EndDate).IsRequired();
            entity.Property(course => course.Schedule).IsRequired().HasMaxLength(100);
            entity.Property(course => course.IsActive).IsRequired();
            entity.Property(course => course.CreatedAt).IsRequired();
            entity.Property(course => course.UpdatedAt).IsRequired();

            // Configure relationships
            entity.HasMany(course => course.Registrations)
                  .WithOne(registration => registration.Course)
                  .HasForeignKey(registration => registration.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Registration entity
        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(registration => registration.RegistrationId);
            entity.Property(registration => registration.StudentId).IsRequired();
            entity.Property(registration => registration.CourseId).IsRequired();
            entity.Property(registration => registration.RegistrationDate).IsRequired();
            entity.Property(registration => registration.Status).IsRequired()
                  .HasConversion<string>();
            entity.Property(registration => registration.Grade).HasConversion<string>();
            entity.Property(registration => registration.Notes).HasMaxLength(200);

            // Create unique constraint to prevent duplicate registrations
            entity.HasIndex(registration => new { registration.StudentId, registration.CourseId })
                  .IsUnique();

            // Configure relationships
            entity.HasOne(registration => registration.Student)
                  .WithMany(student => student.Registrations)
                  .HasForeignKey(registration => registration.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(registration => registration.Course)
                  .WithMany(course => course.Registrations)
                  .HasForeignKey(registration => registration.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Override SaveChanges to automatically update timestamps
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update timestamps
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the timestamps for entities that implement audit fields
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified);

        var currentTime = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Entity is Student student)
            {
                if (entry.State == EntityState.Added)
                {
                    student.CreatedAt = currentTime;
                }
                student.UpdatedAt = currentTime;
            }
            else if (entry.Entity is Course course)
            {
                if (entry.State == EntityState.Added)
                {
                    course.CreatedAt = currentTime;
                }
                course.UpdatedAt = currentTime;
            }
        }
    }
}