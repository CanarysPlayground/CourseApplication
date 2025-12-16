using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using CourseRegistration.Infrastructure.Data;
using CourseRegistration.Infrastructure.Repositories;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Application.Services;
using CourseRegistration.Application.Mappings;
using CourseRegistration.Application.Validators;
using CourseRegistration.API.Middleware;
using System.Reflection;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/course-registration-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Course Registration API", 
        Version = "v1",
        Description = "A comprehensive API for course registration management with clean architecture",
        Contact = new() { Name = "Course Registration Team" }
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add Entity Framework with In-Memory Database
builder.Services.AddDbContext<CourseRegistrationDbContext>(options =>
    options.UseInMemoryDatabase("CourseRegistrationDb"));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();

// Register repositories and unit of work
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Register authorization services
builder.Services.AddScoped<AuthorizationService>();

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add memory cache for performance
builder.Services.AddMemoryCache();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CourseRegistrationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger in all environments for demo purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Registration API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

// Add health check endpoint
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CourseRegistrationDbContext>();
    
    // Ensure database is created
    context.Database.EnsureCreated();
    
    // Seed data if database is empty
    if (!context.Students.Any())
    {
        await SeedDatabase(context);
    }
}

app.Run();

/// <summary>
/// Seeds the database with sample data for testing
/// </summary>
static async Task SeedDatabase(CourseRegistrationDbContext context)
{
    Log.Information("Seeding database with sample data...");
    
    // Sample students
    var students = new[]
    {
        new CourseRegistration.Domain.Entities.Student
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@email.com",
            PhoneNumber = "+1-555-0101",
            DateOfBirth = new DateTime(1995, 5, 15),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Student
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@email.com",
            PhoneNumber = "+1-555-0102",
            DateOfBirth = new DateTime(1993, 8, 22),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Student
        {
            FirstName = "Mike",
            LastName = "Johnson",
            Email = "mike.johnson@email.com",
            PhoneNumber = "+1-555-0103",
            DateOfBirth = new DateTime(1996, 12, 3),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Student
        {
            FirstName = "Sarah",
            LastName = "Williams",
            Email = "sarah.williams@email.com",
            PhoneNumber = "+1-555-0104",
            DateOfBirth = new DateTime(1994, 3, 18),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };
    
    context.Students.AddRange(students);
    await context.SaveChangesAsync();
    
    // Sample courses
    var courses = new[]
    {
        new CourseRegistration.Domain.Entities.Course
        {
            CourseName = "Introduction to Computer Science",
            Description = "Fundamental concepts of computer science including algorithms, data structures, and programming.",
            InstructorName = "Dr. Robert Chen",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(120),
            Schedule = "MWF 9:00-10:30 AM",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Course
        {
            CourseName = "Advanced Mathematics",
            Description = "Advanced topics in calculus, linear algebra, and discrete mathematics.",
            InstructorName = "Prof. Emily Davis",
            StartDate = DateTime.UtcNow.AddDays(45),
            EndDate = DateTime.UtcNow.AddDays(135),
            Schedule = "TTh 2:00-3:30 PM",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Course
        {
            CourseName = "Web Development Fundamentals",
            Description = "Learn HTML, CSS, JavaScript, and modern web development frameworks.",
            InstructorName = "Dr. Michael Brown",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddDays(105),
            Schedule = "MW 6:00-8:00 PM",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Course
        {
            CourseName = "Data Science and Analytics",
            Description = "Introduction to data science, statistics, and machine learning concepts.",
            InstructorName = "Dr. Lisa Anderson",
            StartDate = DateTime.UtcNow.AddDays(60),
            EndDate = DateTime.UtcNow.AddDays(150),
            Schedule = "TTh 10:00-11:30 AM",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new CourseRegistration.Domain.Entities.Course
        {
            CourseName = "Database Design and Management",
            Description = "Comprehensive course on database design, SQL, and database administration.",
            InstructorName = "Prof. David Wilson",
            StartDate = DateTime.UtcNow.AddDays(25),
            EndDate = DateTime.UtcNow.AddDays(115),
            Schedule = "MWF 1:00-2:30 PM",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };
    
    context.Courses.AddRange(courses);
    await context.SaveChangesAsync();
    
    // Sample registrations
    var registrations = new[]
    {
        new CourseRegistration.Domain.Entities.Registration
        {
            StudentId = students[0].StudentId,
            CourseId = courses[0].CourseId,
            Status = CourseRegistration.Domain.Enums.RegistrationStatus.Confirmed,
            RegistrationDate = DateTime.UtcNow.AddDays(-5),
            Notes = "Student is highly motivated and prepared for the course."
        },
        new CourseRegistration.Domain.Entities.Registration
        {
            StudentId = students[1].StudentId,
            CourseId = courses[1].CourseId,
            Status = CourseRegistration.Domain.Enums.RegistrationStatus.Pending,
            RegistrationDate = DateTime.UtcNow.AddDays(-3),
            Notes = "Waiting for prerequisite course completion verification."
        },
        new CourseRegistration.Domain.Entities.Registration
        {
            StudentId = students[0].StudentId,
            CourseId = courses[2].CourseId,
            Status = CourseRegistration.Domain.Enums.RegistrationStatus.Confirmed,
            RegistrationDate = DateTime.UtcNow.AddDays(-2),
            Notes = "Student has relevant programming experience."
        },
        new CourseRegistration.Domain.Entities.Registration
        {
            StudentId = students[2].StudentId,
            CourseId = courses[0].CourseId,
            Status = CourseRegistration.Domain.Enums.RegistrationStatus.Confirmed,
            RegistrationDate = DateTime.UtcNow.AddDays(-4),
            Notes = "First-time computer science student, assigned mentor."
        },
        new CourseRegistration.Domain.Entities.Registration
        {
            StudentId = students[3].StudentId,
            CourseId = courses[3].CourseId,
            Status = CourseRegistration.Domain.Enums.RegistrationStatus.Pending,
            RegistrationDate = DateTime.UtcNow.AddDays(-1),
            Notes = "Student requesting schedule accommodation."
        }
    };
    
    context.Registrations.AddRange(registrations);
    await context.SaveChangesAsync();
    
    Log.Information("Database seeded successfully with {StudentCount} students, {CourseCount} courses, and {RegistrationCount} registrations.", 
        students.Length, courses.Length, registrations.Length);
}

// Make Program accessible for integration tests
public partial class Program { }
