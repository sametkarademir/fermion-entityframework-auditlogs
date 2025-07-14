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
            .Must(x => x == null || x != Guid.Empty);

        RuleFor(x => x.EntityName)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.EntityName), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.MinActivityCount)
            .GreaterThan(0)
            .When(x => x.MinActivityCount.HasValue, ApplyConditionTo.CurrentValidator);
    }
}