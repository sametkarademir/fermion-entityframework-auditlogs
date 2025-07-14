using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class MostModifiedEntitiesRequestDto : DateRangeRequestDto
{
    public List<string>? EntityNames { get; set; }
    public Guid? UserId { get; set; }
}

public class MostModifiedEntitiesRequestValidator : AbstractValidator<MostModifiedEntitiesRequestDto>
{
    public MostModifiedEntitiesRequestValidator()
    {
        RuleFor(x => x.EntityNames)
            .Must(x => x == null || x.Count <= 10)
            .ForEach(x => x.NotEmpty());

        RuleFor(x => x.UserId)
            .Must(x => x == null || x != Guid.Empty)
            .When(x => x.UserId.HasValue, ApplyConditionTo.CurrentValidator);
    }
}