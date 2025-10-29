using FluentValidation;
using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Validators;

/// <summary>
/// Validator for CreateStudentDto
/// </summary>
public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow.AddYears(-16)).WithMessage("Student must be at least 16 years old")
            .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Invalid date of birth");
    }
}

/// <summary>
/// Validator for UpdateStudentDto
/// </summary>
public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow.AddYears(-16)).WithMessage("Student must be at least 16 years old")
            .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Invalid date of birth");
    }
}