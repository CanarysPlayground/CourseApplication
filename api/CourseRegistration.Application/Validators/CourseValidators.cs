using FluentValidation;
using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Validators;

/// <summary>
/// Validator for CreateCourseDto
/// </summary>
public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.InstructorName)
            .NotEmpty().WithMessage("Instructor name is required")
            .MaximumLength(100).WithMessage("Instructor name cannot exceed 100 characters")
            .Matches("^[a-zA-Z\\s\\.]+$").WithMessage("Instructor name can only contain letters, spaces, and periods");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.Schedule)
            .NotEmpty().WithMessage("Schedule is required")
            .MaximumLength(100).WithMessage("Schedule cannot exceed 100 characters");

        // Custom validation to ensure course duration is reasonable
        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays >= 7)
            .WithMessage("Course must be at least 7 days long")
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
            .WithMessage("Course cannot be longer than 365 days");
    }
}

/// <summary>
/// Validator for UpdateCourseDto
/// </summary>
public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.InstructorName)
            .NotEmpty().WithMessage("Instructor name is required")
            .MaximumLength(100).WithMessage("Instructor name cannot exceed 100 characters")
            .Matches("^[a-zA-Z\\s\\.]+$").WithMessage("Instructor name can only contain letters, spaces, and periods");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.Schedule)
            .NotEmpty().WithMessage("Schedule is required")
            .MaximumLength(100).WithMessage("Schedule cannot exceed 100 characters");

        // Custom validation to ensure course duration is reasonable
        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays >= 7)
            .WithMessage("Course must be at least 7 days long")
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
            .WithMessage("Course cannot be longer than 365 days");
    }
}