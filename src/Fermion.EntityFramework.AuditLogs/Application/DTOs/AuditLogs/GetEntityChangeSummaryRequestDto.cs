using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class GetEntityChangeSummaryRequestDto : DataRangeRequestDto
{
    public string EntityId { get; set; } = null!;
    public string EntityName { get; set; } = null!;
}

public class GetEntityChangeSummaryRequestValidator : AbstractValidator<GetEntityChangeSummaryRequestDto>
{
    public GetEntityChangeSummaryRequestValidator()
    {
        RuleFor(x => x.EntityId)
            .NotEmpty().WithMessage("Entity ID cannot be empty.")
            .MaximumLength(100).WithMessage("Entity ID cannot exceed 100 characters.");

        RuleFor(x => x.EntityName)
            .NotEmpty().WithMessage("Entity name cannot be empty.")
            .MaximumLength(500).WithMessage("Entity name cannot exceed 500 characters.");
    }
}