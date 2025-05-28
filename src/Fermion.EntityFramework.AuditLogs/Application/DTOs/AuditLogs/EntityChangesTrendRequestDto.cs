using System.Text.Json.Serialization;
using Fermion.EntityFramework.AuditLogs.Core.Enums;
using FluentValidation;

namespace Fermion.EntityFramework.AuditLogs.Application.DTOs.AuditLogs;

public class EntityChangesTrendRequestDto : DataRangeRequestDto
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
            .NotEmpty().WithMessage("Entity name cannot be empty.")
            .MaximumLength(500).WithMessage("Entity name cannot exceed 500 characters.");

        RuleFor(x => x.PropertyNames)
            .Must(x => x == null || x.Count <= 10).WithMessage("Property names cannot exceed 10 items.")
            .ForEach(x => x.NotEmpty().WithMessage("Property name cannot be empty."));

        RuleFor(x => x.TimeGrouping)
            .IsInEnum().WithMessage("Invalid time grouping value.")
            .Must(x => Enum.IsDefined(typeof(TimeGrouping), x)).WithMessage("Invalid time grouping value.");
    }
}