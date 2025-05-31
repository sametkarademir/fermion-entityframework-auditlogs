using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class UserActivityAnalysisRequestDto : DateRangeRequestDto
{
    public Guid? UserId { get; set; }
    public string? EntityName { get; set; }
    public int? MinActivityCount { get; set; }
}

public class UserActivityAnalysisRequestValidator : AbstractValidator<UserActivityAnalysisRequestDto>
{
    public UserActivityAnalysisRequestValidator()
    {
        RuleFor(x => x.UserId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("User ID cannot be empty if provided.");

        RuleFor(x => x.EntityName)
            .MaximumLength(500).WithMessage("Entity name cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.EntityName), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.MinActivityCount)
            .GreaterThan(0).WithMessage("Minimum activity count must be greater than 0.")
            .When(x => x.MinActivityCount.HasValue, ApplyConditionTo.CurrentValidator);
    }
}