using System.Text.Json.Serialization;
using Fermion.EntityFramework.AuditLogs.Domain.Enums;
using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class EntityChangesTrendRequestDto : DateRangeRequestDto
{
    public string EntityName { get; set; } = null!;
    public List<string>? PropertyNames { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimeGrouping TimeGrouping { get; set; } = TimeGrouping.Daily;
}

public class EntityChangesTrendRequestValidator : AbstractValidator<EntityChangesTrendRequestDto>
{
    public EntityChangesTrendRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.PropertyNames)
            .Must(x => x == null || x.Count <= 10)
            .ForEach(x => x.NotEmpty());

        RuleFor(x => x.TimeGrouping)
            .IsInEnum()
            .Must(x => Enum.IsDefined(typeof(TimeGrouping), x));
    }
}