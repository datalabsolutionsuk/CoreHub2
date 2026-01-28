using CoreHub.Application.DTOs;
using FluentValidation;

namespace CoreHub.Application.Validators;

/// <summary>
/// Validator for CreatePatientDto
/// </summary>
public class CreatePatientDtoValidator : AbstractValidator<CreatePatientDto>
{
    public CreatePatientDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.MobilePhone)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid mobile phone number format")
            .When(x => !string.IsNullOrEmpty(x.MobilePhone));

        RuleFor(x => x.Status)
            .Must(status => new[] { "Active", "Inactive", "Archived" }.Contains(status))
            .WithMessage("Status must be Active, Inactive, or Archived");

        RuleFor(x => x.InsuranceExpiryDate)
            .GreaterThan(DateTime.UtcNow.AddMonths(-1))
            .WithMessage("Insurance expiry date should not be more than 1 month in the past")
            .When(x => x.InsuranceExpiryDate.HasValue);
    }
}
