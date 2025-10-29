using FluentValidation;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.Validators;

/// <summary>
/// Validator for CreateRegistrationDto
/// </summary>
public class CreateRegistrationDtoValidator : AbstractValidator<CreateRegistrationDto>
{
    public CreateRegistrationDtoValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required");

        RuleFor(x => x.Notes)
            .MaximumLength(200).WithMessage("Notes cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

/// <summary>
/// Validator for UpdateRegistrationStatusDto
/// </summary>
public class UpdateRegistrationStatusDtoValidator : AbstractValidator<UpdateRegistrationStatusDto>
{
    public UpdateRegistrationStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid registration status");

        RuleFor(x => x.Grade)
            .IsInEnum().WithMessage("Invalid grade")
            .When(x => x.Grade.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(200).WithMessage("Notes cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        // Grade should only be assigned for completed courses
        RuleFor(x => x.Grade)
            .Null().WithMessage("Grade can only be assigned to completed registrations")
            .When(x => x.Status != RegistrationStatus.Completed);
    }
}