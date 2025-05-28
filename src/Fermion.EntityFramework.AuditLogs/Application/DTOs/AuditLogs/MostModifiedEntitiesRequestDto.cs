using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class MostModifiedEntitiesRequestDto : DataRangeRequestDto
{
    public List<string>? EntityNames { get; set; }
    public Guid? UserId { get; set; }
}

public class MostModifiedEntitiesRequestValidator : AbstractValidator<MostModifiedEntitiesRequestDto>
{
    public MostModifiedEntitiesRequestValidator()
    {
        RuleFor(x => x.EntityNames)
            .Must(x => x == null || x.Count <= 10).WithMessage("Entity names cannot exceed 10 items.")
            .ForEach(x => x.NotEmpty().WithMessage("Entity name cannot be empty."));

        RuleFor(x => x.UserId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("User ID cannot be empty.")
            .When(x => x.UserId.HasValue, ApplyConditionTo.CurrentValidator);
    }
}