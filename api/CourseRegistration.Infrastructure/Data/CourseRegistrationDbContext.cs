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
    /// WaitlistEntries DbSet
    /// </summary>
    public DbSet<WaitlistEntry> WaitlistEntries { get; set; } = null!;

    /// <summary>
    /// Configures the model relationships and constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.StudentId);
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(s => s.LastName).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(256);
            entity.Property(s => s.PhoneNumber).HasMaxLength(20);
            entity.Property(s => s.DateOfBirth).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.UpdatedAt).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();

            // Configure relationships
            entity.HasMany(s => s.Registrations)
                  .WithOne(r => r.Student)
                  .HasForeignKey(r => r.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.WaitlistEntries)
                  .WithOne(w => w.Student)
                  .HasForeignKey(w => w.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Course entity
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.CourseId);
            entity.Property(c => c.CourseName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.InstructorName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.StartDate).IsRequired();
            entity.Property(c => c.EndDate).IsRequired();
            entity.Property(c => c.Schedule).IsRequired().HasMaxLength(100);
            entity.Property(c => c.MaxEnrollment).IsRequired();
            entity.Property(c => c.IsActive).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.UpdatedAt).IsRequired();

            // Configure relationships
            entity.HasMany(c => c.Registrations)
                  .WithOne(r => r.Course)
                  .HasForeignKey(r => r.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.WaitlistEntries)
                  .WithOne(w => w.Course)
                  .HasForeignKey(w => w.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Registration entity
        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(r => r.RegistrationId);
            entity.Property(r => r.StudentId).IsRequired();
            entity.Property(r => r.CourseId).IsRequired();
            entity.Property(r => r.RegistrationDate).IsRequired();
            entity.Property(r => r.Status).IsRequired()
                  .HasConversion<string>();
            entity.Property(r => r.Grade).HasConversion<string>();
            entity.Property(r => r.Notes).HasMaxLength(200);

            // Create unique constraint to prevent duplicate registrations
            entity.HasIndex(r => new { r.StudentId, r.CourseId })
                  .IsUnique();

            // Configure relationships
            entity.HasOne(r => r.Student)
                  .WithMany(s => s.Registrations)
                  .HasForeignKey(r => r.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Course)
                  .WithMany(c => c.Registrations)
                  .HasForeignKey(r => r.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure WaitlistEntry entity
        modelBuilder.Entity<WaitlistEntry>(entity =>
        {
            entity.HasKey(w => w.WaitlistEntryId);
            entity.Property(w => w.StudentId).IsRequired();
            entity.Property(w => w.CourseId).IsRequired();
            entity.Property(w => w.JoinedAt).IsRequired();
            entity.Property(w => w.Position).IsRequired();
            entity.Property(w => w.NotificationPreference).IsRequired()
                  .HasConversion<string>();
            entity.Property(w => w.IsActive).IsRequired();
            entity.Property(w => w.Notes).HasMaxLength(500);

            // Create index for querying active waitlist entries
            // Note: Only one active waitlist entry per student per course should exist
            entity.HasIndex(w => new { w.StudentId, w.CourseId, w.IsActive });

            // Configure relationships
            entity.HasOne(w => w.Student)
                  .WithMany(s => s.WaitlistEntries)
                  .HasForeignKey(w => w.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(w => w.Course)
                  .WithMany(c => c.WaitlistEntries)
                  .HasForeignKey(w => w.CourseId)
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
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

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